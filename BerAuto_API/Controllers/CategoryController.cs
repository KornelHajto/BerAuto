using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BerAuto_API.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly API_DbContext _dbContext;
        private readonly IDistributedCache _cache;
        public CategoryController(API_DbContext dbContext,IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }


        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var cachedCategories = await _cache.GetStringAsync("categories");

            if (!string.IsNullOrEmpty(cachedCategories))
            {
                var categories = JsonConvert.DeserializeObject<List<Category>>(cachedCategories);
                return Ok(categories);
            }

            var categoriesFromDb = await _dbContext.Categories.ToListAsync();

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            var serializedData = JsonConvert.SerializeObject(categoriesFromDb);
            await _cache.SetStringAsync("categories", serializedData, cacheOptions);

            return Ok(categoriesFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            if (category == null) return BadRequest("Invalid category data.");

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            await _cache.RemoveAsync("categories");

            return CreatedAtAction(nameof(GetCategories), new { id = category.ID }, category);
        }
    }
}
