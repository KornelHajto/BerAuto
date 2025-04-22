namespace BerAuto_API.Lib.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> RegisterAsync(User user);
    }
}
