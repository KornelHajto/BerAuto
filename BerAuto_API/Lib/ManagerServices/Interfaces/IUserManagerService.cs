using BerAuto_Lib.DTO;

namespace BerAuto_API.Lib.ManagerServices.Interfaces
{
	public interface IUserManagerService
	{
		Task<string?> AddUserData(UserDataDTO userData);
		Task<string?> DeleteUser(string userId);
		Task<UserViewDTO> GetUser(string userId);
		Task<IEnumerable<UserViewDTO>> ListUsers();
		Task<IEnumerable<UserViewDTO>> Search(string searchTerm);
		Task<string?> UpdateUser(UserUpdateDTO userUpdate);
	}
}
