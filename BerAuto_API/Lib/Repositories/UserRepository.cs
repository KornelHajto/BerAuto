using BerAuto_API.Lib.ManagerServices.Interfaces;
using BerAuto_API.Lib.Repositories.Interfaces;
using BerAuto_Lib.DTO;

namespace BerAuto.Lib.Repositories
{
	public class UserRepository : IUserRepository
	{
		private IServiceScopeFactory scopeFactory;

		public UserRepository(IServiceScopeFactory scopeFactory)
		{
			this.scopeFactory = scopeFactory;
		}

		public Task<string?> AddUserData(UserDataDTO userData)
		{
			var scope = scopeFactory.CreateScope();
			var manager = scope.ServiceProvider.GetRequiredService<IUserManagerService>();
			return manager.AddUserData(userData);
		}

		public Task<string?> DeleteUser(string userId)
		{
			var scope = scopeFactory.CreateScope();
			var manager = scope.ServiceProvider.GetRequiredService<IUserManagerService>();
			return manager.DeleteUser(userId);
		}

		public Task<UserViewDTO> GetUser(string userId)
		{
			var scope = scopeFactory.CreateScope();
			var manager = scope.ServiceProvider.GetRequiredService<IUserManagerService>();
			return manager.GetUser(userId);
		}

		public Task<IEnumerable<UserViewDTO>> ListUsers()
		{
			var scope = scopeFactory.CreateScope();
			var manager = scope.ServiceProvider.GetRequiredService<IUserManagerService>();
			return manager.ListUsers();
		}

		public Task<IEnumerable<UserViewDTO>> Search(string searchTerm)
		{
			var scope = scopeFactory.CreateScope();
			var manager = scope.ServiceProvider.GetRequiredService<IUserManagerService>();
			return manager.Search(searchTerm);
		}

		public Task<string?> UpdateUser(UserUpdateDTO userUpdate)
		{
			var scope = scopeFactory.CreateScope();
			var manager = scope.ServiceProvider.GetRequiredService<IUserManagerService>();
			return manager.UpdateUser(userUpdate);
		}
	}
}