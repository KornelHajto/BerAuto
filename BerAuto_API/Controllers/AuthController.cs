using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BerAuto.DTO;
using BerAuto.Lib.ManagerServices;
using BerAuto_API.Lib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BerAuto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly AuthManagerService _authManager;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                var result = await _unitOfWork.AuthRepository.Register(registerDto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { success = false, message = e.Message });
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var result = await _unitOfWork.AuthRepository.Login(loginDto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { success = false, message = e.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDto)
        {
            try
            {
                var result = await _unitOfWork.AuthRepository.RefreshToken(refreshTokenDto.RefreshToken);
                return Ok(new { success = true, data = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { success = false, message = e.Message });
            }
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult TestAuth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                message = "Authentication successful! Your token is valid.",
                userDetails = new
                {
                    id = userId,
                    name = userName,
                    email = userEmail,
                    role = userRole
                }
            });
        }
    }
}