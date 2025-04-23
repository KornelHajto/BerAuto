namespace BerAuto_API.Lib.ManagerServices.Interfaces
{
    public interface IAuthManagerService
    {
        Task<AuthResponseDTO> Register(RegisterDTO registerDto);
        Task<AuthResponseDTO> Login(LoginDTO loginDto);
        Task<AuthResponseDTO> RefreshToken(string refreshToken);
    }
}