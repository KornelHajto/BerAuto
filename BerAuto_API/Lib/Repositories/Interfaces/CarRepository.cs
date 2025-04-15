

using BerAuto.Lib.ManagerServices;
using BerAuto_API.Lib.ManagerServices.Interfaces;

namespace BerAuto_API.Lib.Repositories.Interfaces
{
	internal class CarRepository : ICarRepository
	{
		public CarRepository(IServiceScopeFactory scopeFactory)
		{
			ScopeFactory = scopeFactory;
		}

		public IServiceScopeFactory ScopeFactory { get; }

		public async Task<CarViewDTO> AppendCarDescription(string id, string description)
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			return await carManager.AppendCarDescription(id, description);
		}

		public async Task CreateCar(Car car)
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			await carManager.CreateCar(car);
		}

		public async Task DeleteCar(string iD)
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			await carManager.DeleteCar(iD);
		}

		public async Task<object?> GetCarDTO(string iD)
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			return await carManager.GetCarDTO(iD);
		}

		public async Task<object?> GetCarRentHistory(string iD)
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			return await carManager.GetCarRentHistory(iD);
		}

		public async Task<bool> IsAvailableOnDayInterval(string iD, DateTime startDate, DateTime endDate)
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			return await carManager.IsAvailableOnDayInterval(iD, startDate, endDate);
		}

		public async Task<object?> ListCars()
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			return await carManager.ListCars();
		}

		public async Task<object?> listCarsWithCategory(string iD)
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			return await carManager.listCarsWithCategory(iD);
		}

		public async Task<CarViewDTO> UpdateCarAvailablity(string id, bool available)
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			return await carManager.UpdateCarAvailablity(id, available);
		}

		public async Task<CarViewDTO> UpdateCarCategory(string carId, string categoryId)
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			return await carManager.UpdateCarCategory(carId, categoryId);
		}

		public async Task<CarViewDTO> UpdateCarOdometer(string id, int odometer)
		{
			var scope = ScopeFactory.CreateScope();
			var carManager = scope.ServiceProvider.GetRequiredService<ICarManagerService>();
			return await carManager.UpdateCarOdometer(id, odometer);
		}
	}
}