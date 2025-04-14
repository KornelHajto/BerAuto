using BerAuto.Lib.ManagerServices;
using BerAuto.Lib.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BerAuto_API.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class CarController : Controller
    {
		CarManagerService carManager;
		public CarController(IUnitOfWork unitOfWork, IDistributedCache cache)
		{
			carManager = new CarManagerService(unitOfWork.DbContext, cache);
		}

		[HttpGet]
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
				response.StatusCode = 400;
				response.Message = e.Message;
				//await $"Error occured: \r\nError code: {response.StatusCode}\r\nError message: {e.Message}".WriteErrorAsync(this._CurrentUser);
			}
			return BadRequest(response);
		}

		[HttpGet("{ID}")]
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
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPost]
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
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPut("category")]
		public async Task<IActionResult> UpdateCarCategory([FromBody] string carId, string categoryId)
		{
			if (carId == null || categoryId == null) return BadRequest("Invalid category data.");
			ApiResponse response = new ApiResponse();
			try
			{
				CarViewDTO car = await carManager.UpdateCarCategory(carId, categoryId);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return AcceptedAtAction(nameof(ListCars), car.ID, car);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPut("odometer")]
		public async Task<IActionResult> UpdateCarOdometer([FromBody] string id, int odometer)
		{
			if (id == null || odometer == null) return BadRequest("Invalid category data.");
			ApiResponse response = new ApiResponse();
			try
			{
				CarViewDTO car = await carManager.UpdateCarOdometer(id, odometer);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return AcceptedAtAction(nameof(ListCars), car.ID, car);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPut("available")]
		public async Task<IActionResult> UpdateCarAvailablity([FromBody] string id, bool available)
		{
			if (id == null || available == null) return BadRequest("Invalid category data.");
			ApiResponse response = new ApiResponse();
			try
			{
				CarViewDTO car = await carManager.UpdateCarAvailablity(id, available);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return AcceptedAtAction(nameof(ListCars), car.ID, car);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpPut("description")]
		public async Task<IActionResult> UpdateCarDescription([FromBody] string id, string description)
		{
			if (id == null || description == null) return BadRequest("Invalid category data.");
			ApiResponse response = new ApiResponse();
			try
			{
				CarViewDTO car = await carManager.AppendCarDescription(id, description);
				//await $"Created new Product: {category.Dump()}".WriteLogAsync(this._CurrentUser);
				return AcceptedAtAction(nameof(ListCars), car.ID, car);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpDelete("{ID}")]
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
				response.StatusCode = 400;
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
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpGet("rentHistory/{ID}")]
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
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		[HttpGet("withCategory/{ID}")]
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
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}
	}
}
