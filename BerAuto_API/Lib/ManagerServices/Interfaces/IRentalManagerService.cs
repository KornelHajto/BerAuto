using BerAuto_Lib.DTO;

namespace BerAuto_API.Lib.ManagerServices.Interfaces
{
	public interface IRentalManagerService
	{
		Task<Guid> CreateRental(NewRentDTO newRent);
		Task<Guid> CreateRentalWithDetails(Guid renterId, Guid carId, DateTime startDate, DateTime endDate);
		Task<CarViewDTO> GetCarForRental(string rentalId);
		Task<CarRentViewDTO> GetRentalDetailsForInvoice(string iD);
		Task<RentViewDTO> GetRentalDTO(string iD);
		Task<IEnumerable<RentViewDTO>> GetUserRentals(string userId);
		Task<IEnumerable<RentViewDTO>> ListRentals();
		Task<IEnumerable<RentViewDTO>> SearchRentals(string searchTerm);
		Task<RentViewDTO> UpdateRentalStatus(string rentalId, ERentStatus newStatus);
	}
}