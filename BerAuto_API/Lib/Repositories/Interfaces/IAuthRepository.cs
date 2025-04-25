namespace BerAuto_API.Lib.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<AuthResponseDTO> Register(RegisterDTO registerDto);
        Task<AuthResponseDTO> Login(LoginDTO loginDto);
        Task<AuthResponseDTO> RefreshToken(string refreshToken);
    }
}