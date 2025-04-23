using BerAuto_API.Lib.ManagerServices.Interfaces;
using BerAuto_API.Lib.Repositories.Interfaces;

namespace BerAuto_API.Lib.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AuthRepository(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<AuthResponseDTO> Register(RegisterDTO registerDto)
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IAuthManagerService>();
            return await service.Register(registerDto);
        }

        public async Task<AuthResponseDTO> Login(LoginDTO loginDto)
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IAuthManagerService>();
            return await service.Login(loginDto);
        }

        public async Task<AuthResponseDTO> RefreshToken(string refreshToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IAuthManagerService>();
            return await service.RefreshToken(refreshToken);
        }
    }
}
