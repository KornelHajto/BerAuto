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
			}
			return BadRequest(response);
		}

		
		[HttpGet("{id}")]
		public async Task<IActionResult> GetCategory(string id) {
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await _unitOfWork.categoryRepository.GetCategory(id);
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
		public async Task<IActionResult> UpdateCategoryName([FromBody] string id, string NewName) {
			if (id == null || NewName == null) return BadRequest("Invalid category data.");
			ApiResponse response = new ApiResponse();
			try
			{
                CategoryViewDTO category = await _unitOfWork.categoryRepository.UpdateCategoryName(id, NewName);
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
		public async Task<IActionResult> UpdateCategoryRate([FromBody] string id, int NewRate)
		{
			if (id == null) return BadRequest("Invalid category data.");
			ApiResponse response = new ApiResponse();
			try
			{
                CategoryViewDTO category = await _unitOfWork.categoryRepository.UpdateCategoryRate(id, NewRate);
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
