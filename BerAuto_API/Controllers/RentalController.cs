using BerAuto.Lib.ManagerServices;
using BerAuto.Models;
using BerAuto_API.Lib.Repositories.Interfaces;
using BerAuto_Lib.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace BerAuto_API.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class RentalController(IUnitOfWork unitofwork) : Controller
    {
        private IUnitOfWork _unitOfWork = unitofwork;

        // kölcsönzések listázása
        [HttpGet]
        public async Task<IActionResult> ListRentals()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.Data = await _unitOfWork.rentalRepository.ListRentals();
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
                response.Data = await _unitOfWork.rentalRepository.GetRental(ID);
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
        public async Task<IActionResult> CreateRental([FromBody] NewRentDTO newRent)
        {
            if (newRent == null) return BadRequest("Invalid rental data.");

            ApiResponse response = new ApiResponse();
            try
            {
                var rentalId = await _unitOfWork.rentalRepository.CreateRental(newRent);
				return CreatedAtAction(nameof(ListRentals), new { id = rentalId }, newRent);
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
                var rentalId = await _unitOfWork.rentalRepository.CreateRentalWithDetails(
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
                var rental = await _unitOfWork.rentalRepository.UpdateRentalStatus(updateData.RentalId, updateData.NewStatus);
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
                response.Data = await _unitOfWork.rentalRepository.GetRentalDetailsForInvoice(ID);
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
                response.Data = await _unitOfWork.rentalRepository.SearchRentals(searchTerm);
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
                response.Data = await _unitOfWork.rentalRepository.GetCarForRental(rentalId);
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
                response.Data = await _unitOfWork.rentalRepository.GetUserRentals(userId);
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