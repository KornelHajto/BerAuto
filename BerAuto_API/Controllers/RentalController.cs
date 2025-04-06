using BerAuto.Lib.ManagerServices;
using BerAuto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace BerAuto_API.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class RentalController : Controller
    {
        private readonly RentalManagerService _rentalManager;

        public RentalController(API_DbContext dbContext, IDistributedCache cache)
        {
            _rentalManager = new RentalManagerService(dbContext, cache);
        }

        // kölcsönzések listázása
        [HttpGet]
        public async Task<IActionResult> ListRentals()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.Data = await _rentalManager.ListRentals();
                return Ok(response);
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                response.Message = e.Message;
            }
            return BadRequest(response);
        }
        
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetRental(string ID)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.Data = await _rentalManager.GetRentalDTO(ID);
                return Ok(response);
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                response.Message = e.Message;
            }
            return BadRequest(response);
        }

        // kölcsönzés létrehozása
        [HttpPost]
        public async Task<IActionResult> CreateRental([FromBody] Rent rental)
        {
            if (rental == null) return BadRequest("Invalid rental data.");

            ApiResponse response = new ApiResponse();
            try
            {
                var rentalId = await _rentalManager.CreateRental(rental);
                return CreatedAtAction(nameof(ListRentals), new { id = rentalId }, rental);
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                response.Message = e.Message;
            }
            return BadRequest(response);
        }

        // kölcsönzés létrehozása + részletek megadása
        [HttpPost("withDetails")]
        public async Task<IActionResult> CreateRentalWithDetails([FromBody] RentalCreateDTO rentalData)
        {
            if (rentalData == null) return BadRequest("Invalid rental data.");

            ApiResponse response = new ApiResponse();
            try
            {
                var rentalId = await _rentalManager.CreateRentalWithDetails(
                    rentalData.RenterId, 
                    rentalData.CarId, 
                    rentalData.StartDate, 
                    rentalData.EndDate);
                
                return CreatedAtAction(nameof(GetRental), new { ID = rentalId }, rentalId);
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                response.Message = e.Message;
            }
            return BadRequest(response);
        }

        // kölcsönzés módosítása (státusz)
        [HttpPut("status")]
        public async Task<IActionResult> UpdateRentalStatus([FromBody] RentalStatusUpdateDTO updateData)
        {
            if (updateData == null) return BadRequest("Invalid update data.");

            ApiResponse response = new ApiResponse();
            try
            {
                var rental = await _rentalManager.UpdateRentalStatus(updateData.RentalId, updateData.NewStatus);
                return AcceptedAtAction(nameof(GetRental), new { ID = updateData.RentalId }, rental);
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                response.Message = e.Message;
            }
            return BadRequest(response);
        }

        // egy kölcsönzés részleteinek megadása (számla létrehozásához)
        [HttpGet("invoice/{ID}")]
        public async Task<IActionResult> GetRentalInvoiceDetails(string ID)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.Data = await _rentalManager.GetRentalDetailsForInvoice(ID);
                return Ok(response);
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                response.Message = e.Message;
            }
            return BadRequest(response);
        }

        // kölcsönzések keresése
        [HttpGet("search")]
        public async Task<IActionResult> SearchRentals([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest("Search term is required.");

            ApiResponse response = new ApiResponse();
            try
            {
                response.Data = await _rentalManager.SearchRentals(searchTerm);
                return Ok(response);
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                response.Message = e.Message;
            }
            return BadRequest(response);
        }

        // kölcsönzéshez tartozó autó adatainak megtekintése
        [HttpGet("{rentalId}/car")]
        public async Task<IActionResult> GetRentalCar(string rentalId)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.Data = await _rentalManager.GetCarForRental(rentalId);
                return Ok(response);
            }
            catch (Exception e)
            {
                response.StatusCode = 400;
                response.Message = e.Message;
            }
            return BadRequest(response);
        }

        // egy felhasználóhoz tartozó kölcsönzések kilistázása
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRentals(string userId)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.Data = await _rentalManager.GetUserRentals(userId);
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