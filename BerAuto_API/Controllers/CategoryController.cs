using BerAuto.Lib.ManagerServices;
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
        //private readonly API_DbContext _dbContext;
        //private readonly IDistributedCache _cache;
        CategoryManagerService categoryManager;
        public CategoryController(API_DbContext dbContext,IDistributedCache cache)
        {
            categoryManager = new CategoryManagerService(dbContext, cache);
        }


        [HttpGet("ListCategories")]
        public async Task<IActionResult> ListCategories()
        {
            
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await categoryManager.ListCategories();
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 203;
				response.Message = e.Message;
				//await $"Error occured: \r\nError code: {response.StatusCode}\r\nError message: {e.Message}".WriteErrorAsync(this._CurrentUser);
			}
			return BadRequest(response);
		}

		
		[HttpGet("GetCategory/{ID}")]
		public async Task<IActionResult> GetCategory(string ID) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await categoryManager.GetCategory(ID);
				return Ok(response);
			}
			catch (Exception e) {
				response.StatusCode = 201;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}
		

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
			if (category == null) return BadRequest("Invalid category data.");

			ApiResponse response = new ApiResponse();
            try
            {
                await categoryManager.CreateCategory(category);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return CreatedAtAction(nameof(ListCategories), new { id = category.ID }, category);
			}
			catch (Exception e)
			{
				response.StatusCode = 202;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPost("UpdateCategory")]
		public async Task<IActionResult> UpdateCategory([FromBody] Category category) {
			if (category == null) return BadRequest("Invalid category data.");
			ApiResponse response = new ApiResponse();
			try
			{
				await categoryManager.UpdateCategory(category);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return AcceptedAtAction(nameof(ListCategories), category.ID , category);
			}
			catch (Exception e)
			{
				response.StatusCode = 202;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}
    }
}
