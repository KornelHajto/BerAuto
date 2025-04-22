using BerAuto_API.Lib.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class AuthRepository : IAuthRepository
{
    private readonly API_DbContext _context;

    public AuthRepository(IServiceScopeFactory scopeFactory)
    {
        _context = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<API_DbContext>();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
    }

    public async Task<bool> RegisterAsync(User user)
    {
        _context.Users.Add(user);
        return await _context.SaveChangesAsync() > 0;
    }
}
