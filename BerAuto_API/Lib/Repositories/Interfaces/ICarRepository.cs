namespace BerAuto_API.Lib.Repositories.Interfaces
{
	public interface ICarRepository
	{
		Task<CarViewDTO> AppendCarDescription(string id, string description);
		Task CreateCar(Car car);
		Task DeleteCar(string iD);
		Task<object?> GetCarDTO(string iD);
		Task<object?> GetCarRentHistory(string iD);
		Task<bool> IsAvailableOnDayInterval(string iD, DateTime startDate, DateTime endDate);
		Task<object?> ListCars();
		Task<object?> listCarsWithCategory(string iD);
		Task<CarViewDTO> UpdateCarAvailablity(string id, bool available);
		Task<CarViewDTO> UpdateCarCategory(string carId, string categoryId);
		Task<CarViewDTO> UpdateCarOdometer(string id, int odometer);
	}
}