using BerAuto_API.Lib.ManagerServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BerAuto.Lib.ManagerServices
{
	public class CategoryManagerService :ICategoryManagerService
	{
		private readonly API_DbContext _dbContext;
		private readonly IDistributedCache _cache;
		public CategoryManagerService(API_DbContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}

        //public async Task<IEnumerable<Category>> ListCategories()
        //{
        //	var categories = await getCategoriesCache();
        //	if (categories == null)
        //	{
        //		categories = await getCategoriesDB();
        //	}
        //	if (categories == null)
        //	{
        //		return null;
        //	}
        //	return categories.ToList();
        //}
        public async Task<IEnumerable<CategoryViewDTO>> ListCategories()
        {
            var categories = await getCategoriesCache();
            if (categories == null)
            {
                categories = await getCategoriesDB();
            }
            if (categories == null)
            {
                return null;
            }
            return categories.Adapt<List<CategoryViewDTO>>();
        }

        //public async Task<Category> GetCategory(string ID) {
        //	var categories = await getCategoriesCache();
        //	if (categories == null)
        //	{
        //		categories = await getCategoriesDB();
        //	}
        //	if (categories == null)
        //	{
        //		return null;
        //	}
        //	return categories.Where(c => c.ID.ToString().Equals(ID)).FirstOrDefault();
        //}
        public async Task<CategoryViewDTO> GetCategory(string ID)
        {
            var categories = await getCategoriesCache() ?? await getCategoriesDB();
            var category = categories.FirstOrDefault(c => c.ID.ToString().Equals(ID));
            return category?.Adapt<CategoryViewDTO>();
        }
        public async Task<Category> GetCategoryEntity(string ID)
        {
            var categories = await getCategoriesCache() ?? await getCategoriesDB();
            return categories.FirstOrDefault(c => c.ID.ToString().Equals(ID));
        }



        public async Task CreateCategory(Category category) {
			_dbContext.Categories.Add(category);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("categories");
		}

        //public async Task<Category> UpdateCategoryName(string ID, string NewName)
        //{
        //	bool exists = await doesCategoryExists(ID);
        //	if(!exists) throw new Exception("Category does not exist");

        //	var category = await _dbContext.Categories.Where(c => c.ID.ToString().Equals(ID)).FirstOrDefaultAsync();
        //	category.Name = NewName;
        //	await UpdateCategory(category);
        //	return category;
        //}

        //public async Task<Category> UpdateCategoryRate(string ID, int NewRate)
        //{
        //	bool exists = await doesCategoryExists(ID);
        //	if (!exists) throw new Exception("Category does not exist");

        //	var category = await _dbContext.Categories.Where(c => c.ID.ToString().Equals(ID)).FirstOrDefaultAsync();
        //	category.DailyRate = NewRate;
        //	await UpdateCategory(category);
        //	return category;
        //}
        public async Task<CategoryViewDTO> UpdateCategoryName(string ID, string NewName)
        {
            if (!await doesCategoryExists(ID)) throw new Exception("Category does not exist");

            var category = await _dbContext.Categories.Where(c => c.ID.ToString().Equals(ID)).FirstOrDefaultAsync();
            category.Name = NewName;
            await UpdateCategory(category);
            return category.Adapt<CategoryViewDTO>();
        }

        public async Task<CategoryViewDTO> UpdateCategoryRate(string ID, int NewRate)
        {
            if (!await doesCategoryExists(ID)) throw new Exception("Category does not exist");

            var category = await _dbContext.Categories.Where(c => c.ID.ToString().Equals(ID)).FirstOrDefaultAsync();
            category.DailyRate = NewRate;
            await UpdateCategory(category);
            return category.Adapt<CategoryViewDTO>();
        }

        public async Task<bool> doesCategoryExists(string ID)
		{
			var category = await _dbContext.Categories.Where(c => c.ID.Equals(ID)).FirstOrDefaultAsync();
			return category != null;
		}

		//Private methods:
		private async Task<IQueryable<Category>> getCategoriesCache()
		{
			var cachedCategories = await _cache.GetStringAsync("categories");
			if (!string.IsNullOrEmpty(cachedCategories))
			{
				var category = JsonConvert.DeserializeObject<List<Category>>(cachedCategories);
				return category.AsQueryable();
			}
			return null;
		}

		private async Task<IQueryable<Category>> getCategoriesDB()
		{
			var categoriesFromDb = await _dbContext.Categories.OrderBy(c => c.ID).ToListAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};

			var serializedData = JsonConvert.SerializeObject(categoriesFromDb);
			await _cache.SetStringAsync("categories", serializedData, cacheOptions);
			return categoriesFromDb.AsQueryable();
		}

		private async Task UpdateCategory(Category category)
		{
			_dbContext.Categories.Update(category);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("categories");
		}

		

	}
}
