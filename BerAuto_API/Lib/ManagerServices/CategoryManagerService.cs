using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BerAuto.Lib.ManagerServices
{
	public class CategoryManagerService
	{
		private readonly API_DbContext _dbContext;
		private readonly IDistributedCache _cache;
		public CategoryManagerService(API_DbContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}

		public async Task<Category> GetCategory(string ID) {
			var cachedCategories = await _cache.GetStringAsync("categories");
			if (!string.IsNullOrEmpty(cachedCategories))
			{
				var category = JsonConvert.DeserializeObject<List<Category>>(cachedCategories).Where(c=> c.ID.Equals(ID)).FirstOrDefault();
				return category;
			}
			var categoryFromDb = await _dbContext.Categories.Where(c => c.ID.Equals(ID)).FirstOrDefaultAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};

			var categoriesFromDb = await _dbContext.Categories.OrderBy(c => c.ID).ToListAsync();
			var serializedData = JsonConvert.SerializeObject(categoriesFromDb);
			await _cache.SetStringAsync("categories", serializedData, cacheOptions);


			return categoryFromDb;
		}
		

		public async Task<IEnumerable<Category>> ListCategories()
		{
			var cachedCategories = await _cache.GetStringAsync("categories");
			if (!string.IsNullOrEmpty(cachedCategories))
			{
				var categories = JsonConvert.DeserializeObject<List<Category>>(cachedCategories);
				return categories;
			}

			var categoriesFromDb = await _dbContext.Categories.OrderBy(c => c.ID).ToListAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};

			var serializedData = JsonConvert.SerializeObject(categoriesFromDb);
			await _cache.SetStringAsync("categories", serializedData, cacheOptions);

			return categoriesFromDb;

		}

		public async Task CreateCategory(Category category) {
			_dbContext.Categories.Add(category);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("categories");
		}

		public async Task UpdateCategory(Category category) {
			_dbContext.Categories.Update(category);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("categories");
		}

	}
}
