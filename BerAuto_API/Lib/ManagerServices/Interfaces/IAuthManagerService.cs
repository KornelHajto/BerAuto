namespace BerAuto.Lib.ManagerServices
{
    public interface IAuthManagerService
    {
        Task<string> Login(LoginDTO loginDto);
        Task<bool> Register(RegisterDTO registerDto);
        Task<string> RefreshToken(string refreshToken);
    }
}