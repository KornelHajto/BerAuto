using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BerAuto.DTO;
using BerAuto.Enums;
using BerAuto.Models;
using BerAuto_API.Lib.ManagerServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BerAuto.Lib.ManagerServices
{
    public class AuthManagerService : IAuthManagerService
    {
        private readonly API_DbContext _context;
        private readonly IConfiguration _configuration;

        public AuthManagerService(API_DbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> Login(LoginDTO loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Enabled);

            if (user == null || !VerifyPassword(loginDto.Password, user.Password))
                throw new Exception("Invalid email or password");

            return await GenerateTokensForUser(user);
        }

        public async Task<AuthResponseDTO> Register(RegisterDTO registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                throw new Exception("Email already registered");

            var user = new User
            {
                ID = Guid.NewGuid(),
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = HashPassword(registerDto.Password),
                Address = registerDto.Address,
                PhoneNumber = registerDto.PhoneNumber,
                Description = registerDto.Description,
                AccesLevel = EUserType.User,
                Enabled = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await GenerateTokensForUser(user);
        }

        public async Task<AuthResponseDTO> RefreshToken(string token)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == token && u.RefreshTokenExpires > DateTime.UtcNow && u.Enabled);

            if (user == null)
                throw new Exception("Invalid or expired refresh token");

            return await GenerateTokensForUser(user);
        }

        private async Task<AuthResponseDTO> GenerateTokensForUser(User user)
        {
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpires = DateTime.UtcNow.AddDays(
                Convert.ToDouble(_configuration["JwtSettings:RefreshTokenExpiryDays"]));

            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["JwtSettings:AccessTokenExpiryMinutes"])),
                UserId = user.ID,
                Name = user.Name,
                Email = user.Email,
                AccesLevel = user.AccesLevel.ToString()
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.AccesLevel.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(
                    _configuration["JwtSettings:AccessTokenExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}