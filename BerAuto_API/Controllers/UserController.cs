using BerAuto_API.Lib.Repositories.Interfaces;
using BerAuto_Lib.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BerAuto_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController(IUnitOfWork unitofwork) : Controller
    {
		private IUnitOfWork _unitOfWork = unitofwork;

		//felhasználók listázása
		[HttpGet]
		public async Task<IActionResult> ListUsers()
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await _unitOfWork.userRepository.ListUsers();
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		//egy felhasználó adatainak megtekintése
		[HttpGet("{userId}")]
		public async Task<IActionResult> GetUser(string userId)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await _unitOfWork.userRepository.GetUser(userId);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		//felhasználó adatainak módosítása
		[HttpPut]
		public async Task<IActionResult> UpdateUser(UserUpdateDTO userUpdate)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Message = await _unitOfWork.userRepository.UpdateUser(userUpdate);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		//felhasználó adatainak megadása
		[HttpPut]
		public async Task<IActionResult> AddUserData(UserDataDTO userData)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Message = await _unitOfWork.userRepository.AddUserData(userData);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		//felhasználó törlése
		[HttpDelete("{userId}")]
		public async Task<IActionResult> DeleteUser(string userId)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Message = await _unitOfWork.userRepository.DeleteUser(userId);
				return Ok(response);
			}
			catch (Exception e)
			{
				response.StatusCode = 400;
				response.Message = e.Message;
			}
			return BadRequest(response);
		}

		//felhasználó keresése
		[HttpGet("search/{searchterm}")]
		public async Task<IActionResult> SearchUser(string searchTerm)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				response.Data = await _unitOfWork.userRepository.Search(searchTerm);
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
