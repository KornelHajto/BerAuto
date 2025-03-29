using BerAuto.Lib.ManagerServices;
using Microsoft.AspNetCore.Mvc;

namespace BerAuto_API.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class CarController : Controller
    {
		CarManagerService carManager;
		public CarController(API_DbContext dbContext, IDistributedCache cache)
		{
			carManager = new CarManagerService(dbContext, cache);
		}

		[HttpGet("ListCars")]
		public async Task<IActionResult> ListCars()
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await carManager.ListCars();
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

		[HttpGet("GetCar/{ID}")]
		public async Task<IActionResult> GetCar(string ID)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await carManager.GetCarDTO(ID);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 201;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPost("CreateCar")]
		public async Task<IActionResult> CreateCar([FromBody] Car car)
		{
			if (car == null) return BadRequest("Invalid category data.");

			ApiResponse response = new ApiResponse();
			try
			{
				await carManager.CreateCar(car);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return CreatedAtAction(nameof(ListCars), new { id = car.ID }, car);
			}
			catch (Exception e)
			{
				response.StatusCode = 202;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPut("UpdateCar")]
		public async Task<IActionResult> UpdateCar([FromBody] Car car)
		{
			if (car == null) return BadRequest("Invalid category data.");
			ApiResponse response = new ApiResponse();
			try
			{
				await carManager.UpdateCar(car);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return AcceptedAtAction(nameof(ListCars), car.ID, car);
			}
			catch (Exception e)
			{
				response.StatusCode = 202;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpDelete("DeleteCar/{ID}")]
		public async Task<IActionResult> DeleteCar(string ID)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				await carManager.DeleteCar(ID);
				response.Message = "Car deleted successfully";
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 202;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpGet("IsAvailable/{ID}")] //not working properly throws: 404 not found
		public async Task<IActionResult> IsAvailable([FromBody] string ID, DateTime startDate, DateTime endDate)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = carManager.IsAvailableOnDayInterval(ID, startDate, endDate);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 202;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpGet("CarRentHistory/{ID}")]
		public async Task<IActionResult> CarRentHistory(string ID)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await carManager.GetCarRentHistory(ID);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 202;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpGet("GetCarsWithCategory/{ID}")]
		public async Task<IActionResult> GetCarWithCategory(string ID)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await carManager.listCarsWithCategory(ID);
				return Ok(response);
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
