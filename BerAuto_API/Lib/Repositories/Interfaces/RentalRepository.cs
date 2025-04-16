
using BerAuto.Lib.ManagerServices;
using BerAuto_API.Lib.ManagerServices.Interfaces;

namespace BerAuto_API.Lib.Repositories.Interfaces
{
	public class RentalRepository : IRentalRepository
	{
		private readonly IServiceScopeFactory _scopeFactory;
		public RentalRepository(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}

		public async Task<Guid> CreateRental(Rent rental)
		{
			var scope = _scopeFactory.CreateScope();
			var rentalManager = scope.ServiceProvider.GetRequiredService<IRentalManagerService>();
			return await rentalManager.CreateRental(rental);
		}

		public async Task<Guid> CreateRentalWithDetails(Guid renterId, Guid carId, DateTime startDate, DateTime endDate)
		{
			var scope = _scopeFactory.CreateScope();
			var rentalManager = scope.ServiceProvider.GetRequiredService<IRentalManagerService>();
			return await rentalManager.CreateRentalWithDetails(renterId, carId, startDate, endDate);
		}

		public async Task<object?> GetCarForRental(string rentalId)
		{
			var scope = _scopeFactory.CreateScope();
			var rentalManager = scope.ServiceProvider.GetRequiredService<IRentalManagerService>();
			return await rentalManager.GetCarForRental(rentalId);
		}

		public async Task<object?> GetRental(string iD)
		{
			var scope = _scopeFactory.CreateScope();
			var rentalManager = scope.ServiceProvider.GetRequiredService<IRentalManagerService>();
			return await rentalManager.GetRentalDTO(iD);
		}

		public async Task<object?> GetRentalDetailsForInvoice(string iD)
		{
			var scope = _scopeFactory.CreateScope();
			var rentalManager = scope.ServiceProvider.GetRequiredService<IRentalManagerService>();
			return await rentalManager.GetRentalDetailsForInvoice(iD);
		}

		public async Task<object?> GetUserRentals(string userId)
		{
			var scope = _scopeFactory.CreateScope();
			var rentalManager = scope.ServiceProvider.GetRequiredService<IRentalManagerService>();
			return await rentalManager.GetUserRentals(userId);
		}

		public async Task<object?> ListRentals()
		{
			var scope = _scopeFactory.CreateScope();
			var rentalManager = scope.ServiceProvider.GetRequiredService<IRentalManagerService>();
			return await rentalManager.ListRentals();
		}

		public async Task<object?> SearchRentals(string searchTerm)
		{
			var scope = _scopeFactory.CreateScope();
			var rentalManager = scope.ServiceProvider.GetRequiredService<IRentalManagerService>();
			return await rentalManager.SearchRentals(searchTerm);
		}

		public async Task<RentViewDTO> UpdateRentalStatus(string rentalId, ERentStatus newStatus)
		{
			var scope = _scopeFactory.CreateScope();
			var rentalManager = scope.ServiceProvider.GetRequiredService<IRentalManagerService>();
			return await rentalManager.UpdateRentalStatus(rentalId, newStatus);
		}
	}
}
