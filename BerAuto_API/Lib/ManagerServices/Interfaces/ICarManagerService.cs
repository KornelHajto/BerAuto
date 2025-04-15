


namespace BerAuto_API.Lib.ManagerServices.Interfaces
{
	public interface ICarManagerService
	{
		Task<CarViewDTO> AppendCarDescription(string id, string description);
		Task CreateCar(Car car);
		Task DeleteCar(string iD);
		Task<CarViewDTO> GetCarDTO(string iD);
		Task<IEnumerable<CarRentViewDTO>> GetCarRentHistory(string iD);
		Task<bool> IsAvailableOnDayInterval(string iD, DateTime startDate, DateTime endDate);
		Task<IEnumerable<CarViewDTO>> ListCars();
		Task<IEnumerable<CarViewDTO>> listCarsWithCategory(string iD);
		Task<CarViewDTO> UpdateCarAvailablity(string id, bool available);
		Task<CarViewDTO> UpdateCarCategory(string carId, string categoryId);
		Task<CarViewDTO> UpdateCarOdometer(string id, int odometer);
	}
}