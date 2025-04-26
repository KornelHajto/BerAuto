using BerAuto_Lib.DTO;

namespace BerAuto_API.Lib.Repositories.Interfaces
{
	public interface IRentalRepository
	{
		Task<object?> ListRentals();
		Task<object?> GetRental(string iD);
		Task<Guid> CreateRental(NewRentDTO newRental);
		Task<Guid> CreateRentalWithDetails(Guid renterId, Guid carId, DateTime startDate, DateTime endDate);
		Task<RentViewDTO> UpdateRentalStatus(string rentalId, ERentStatus newStatus);
		Task<object?> GetRentalDetailsForInvoice(string iD);
		Task<object?> SearchRentals(string searchTerm);
		Task<object?> GetCarForRental(string rentalId);
		Task<object?> GetUserRentals(string userId);
	}
}
