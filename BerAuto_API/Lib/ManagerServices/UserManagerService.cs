using BerAuto_API.Lib.ManagerServices.Interfaces;
using BerAuto_Lib.DTO;
using Mapster;
using Newtonsoft.Json;

namespace BerAuto_API.Lib.ManagerServices
{
	public class UserManagerService : IUserManagerService
	{
		private readonly API_DbContext _dbContext;
		private readonly IDistributedCache _cache;
		public UserManagerService(API_DbContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}

		private async Task<IQueryable<User>> querryUsers()
		{
			var cachedUsers = await _cache.GetStringAsync("users");
			if (!string.IsNullOrEmpty(cachedUsers))
			{
				var user = JsonConvert.DeserializeObject<List<User>>(cachedUsers);
				return user.AsQueryable();
			}
			var usersFromDb = await _dbContext.Users.OrderBy(c => c.ID).ToListAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};

			var serializedData = JsonConvert.SerializeObject(usersFromDb);
			await _cache.SetStringAsync("users", serializedData, cacheOptions);
			return usersFromDb.AsQueryable();
		}

		public async Task<IEnumerable<UserViewDTO>> ListUsers()
		{
			var users = await querryUsers();
			if (users == null) throw new Exception("No users found");
			users = users.Where(c => c.Enabled == true);
			var usersList = users.Adapt<List<UserViewDTO>>();
			return usersList;
		}

		public async Task<UserViewDTO> GetUser(string userId)
		{
			var users = await querryUsers();
			var user = users.FirstOrDefault(c => c.ID.ToString().Equals(userId));
			if (user == null) throw new Exception("User not found");
			if (user.Enabled == false) throw new Exception("User is deleted");
			return user.Adapt<UserViewDTO>();
		}

		public async Task<string?> AddUserData(UserDataDTO userData)
		{
			var users = await querryUsers();
			var user = users.FirstOrDefault(c => c.ID.ToString().Equals(userData.UserID));
			if (user == null) throw new Exception("User not found");
			if (user.Enabled == false) throw new Exception("User is deleted");
			if( (user.Name != null && userData.Override) || user.Name == null || user.Name == string.Empty)
			{
				user.Name = userData.Name;
			}
			if ((user.Email != null && userData.Override) || user.Email == null || user.Email == string.Empty)
			{
				user.Email = userData.Email;
			}
			if ((user.Address != null && userData.Override) || user.Address == null || user.Address == string.Empty)
			{
				user.Address = userData.Address;
			}
			if ((user.PhoneNumber != null && userData.Override) || user.PhoneNumber == null || user.PhoneNumber == string.Empty)
			{
				user.PhoneNumber = userData.PhoneNumber;
			}
			if ((user.Description != null && userData.Override) || user.Description == null || user.Description == string.Empty)
			{
				user.Description = userData.Description;
			}
			await UpdateUser(user);
			return "User data added successfully";
		}

		public async Task<string?> DeleteUser(string userId)
		{
			var users = await querryUsers();
			if (users == null) throw new Exception("No users found");
			var user = users.FirstOrDefault(c => c.ID.ToString().Equals(userId));
			if (user == null) throw new Exception("User not found");
			if (user.Enabled == false) throw new Exception("User is already deleted");
			user.Enabled = false;
			await UpdateUser(user);
			return "User deleted successfully";
		}

		public async Task<IEnumerable<UserViewDTO>> Search(string searchTerm)
		{
			var users = await querryUsers();
			if (users == null) throw new Exception("No users found");
			var filteredUsers = users.Where(c =>
				c.ID.ToString().ToLower().Contains(searchTerm) ||
				c.Name.ToLower().Contains(searchTerm) ||
				c.Email.ToLower().Contains(searchTerm) ||
				c.Address.ToLower().Contains(searchTerm) ||
				c.Description.ToLower().Contains(searchTerm)
			);
			if (filteredUsers == null) throw new Exception("No users found");
			var filteredUsersList = filteredUsers.Adapt<List<UserViewDTO>>();
			return filteredUsersList;
		}

		public async Task<string?> UpdateUser(UserUpdateDTO userUpdate)
		{
			var users = await querryUsers();
			if (users == null) throw new Exception("No users found");
			var user = users.FirstOrDefault(c => c.ID.ToString().Equals(userUpdate.UserId));
			if (user == null) throw new Exception("User not found");
			if (user.Enabled == false) throw new Exception("User is deleted");
			switch (userUpdate.PropertyName)
			{
				case "Name":
					user.Name = userUpdate.Value;
					break;
				case "Email":
					user.Email = userUpdate.Value;
					break;
				case "Address":
					user.Address = userUpdate.Value;
					break;
				case "PhoneNumber":
					user.PhoneNumber = userUpdate.Value;
					break;
				case "Description":
					user.Description = userUpdate.Value;
					break;
				case "AccesLevel":
					if (Enum.TryParse<EUserType>(userUpdate.Value, out var accesLevel))
					{
						user.AccesLevel = accesLevel;
					}
					else
					{
						throw new Exception("Invalid acces level");
					}
					break;
				case "RefreshToken":
					user.RefreshToken = userUpdate.Value;
					break;
				case "RefreshTokenExpires":
					if (DateTime.TryParse(userUpdate.Value, out var refreshTokenExpires))
					{
						user.RefreshTokenExpires = refreshTokenExpires;
					}
					else
					{
						throw new Exception("Invalid refresh token expires");
					}
					break;
				default:
					throw new Exception("Invalid property name");
			}
			await UpdateUser(user);
			return "User updated successfully: " + userUpdate.PropertyName + " = " + userUpdate.Value + " for user: " + userUpdate.UserId;
		}

		public async Task UpdateUser(User user)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				_dbContext.Users.Update(user);
				await _dbContext.SaveChangesAsync();
				_cache.Remove("users");
				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error updating user", ex);
			}
			finally
			{
				await transaction.DisposeAsync();
			}
		}
	}
}
