using BerAuto.Lib.ManagerServices;
using BerAuto_API.Lib.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace BerAuto_API.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CategoryController(IUnitOfWork unitofwork) : Controller
    {
		private IUnitOfWork _unitOfWork = unitofwork;

		[HttpGet]
        public async Task<IActionResult> ListCategories()
        {
            
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await _unitOfWork.categoryRepository.ListCategories();
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
				//await $"Error occured: \r\nError code: {response.StatusCode}\r\nError message: {e.Message}".WriteErrorAsync(this._CurrentUser);
			}
			return BadRequest(response);
		}

		
		[HttpGet("{ID}")]
		public async Task<IActionResult> GetCategory(string ID) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await _unitOfWork.categoryRepository.GetCategory(ID);
				return Ok(response);
			}
			catch (Exception e) {
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}
		

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] string name, int dailyRate)
        {
			if (name == null) return BadRequest("Invalid category data.");

			ApiResponse response = new ApiResponse();
            try
            {
				Category c = new Category();
				c.Name = name;
				c.DailyRate = dailyRate;
				await _unitOfWork.categoryRepository.CreateCategory(c);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return CreatedAtAction(nameof(ListCategories), new { id = c.ID }, c);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPut("name")]
		public async Task<IActionResult> UpdateCategoryName([FromBody] string ID, string NewName) {
			if (ID == null || NewName == null) return BadRequest("Invalid category data.");
			ApiResponse response = new ApiResponse();
			try
			{
                CategoryViewDTO category = await _unitOfWork.categoryRepository.UpdateCategoryName(ID, NewName);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return AcceptedAtAction(nameof(ListCategories), category.ID , category);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPut("rate")]
		public async Task<IActionResult> UpdateCategoryRate([FromBody] string ID, int NewRate)
		{
			if (ID == null) return BadRequest("Invalid category data.");
			ApiResponse response = new ApiResponse();
			try
			{
                CategoryViewDTO category = await _unitOfWork.categoryRepository.UpdateCategoryRate(ID, NewRate);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return AcceptedAtAction(nameof(ListCategories), category.ID, category);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}
	}
}
