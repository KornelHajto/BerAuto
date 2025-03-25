using BerAuto.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BerAuto.Lib.ManagerServices
{
	public class CarManagerService
	{
		private readonly API_DbContext _dbContext;
		private readonly IDistributedCache _cache;
		public CarManagerService(API_DbContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;
		}

		public async Task<IEnumerable<Car>> ListCars() 
		{
			var cachedCars = await _cache.GetStringAsync("cars");
			if (!string.IsNullOrEmpty(cachedCars))
			{
				var cars = JsonConvert.DeserializeObject<List<Car>>(cachedCars);
				return cars;
			}

			var carsfFromDb = await _dbContext.Cars.OrderBy(c => c.ID).ToListAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};

			var serializedData = JsonConvert.SerializeObject(carsfFromDb);
			await _cache.SetStringAsync("cars", serializedData, cacheOptions);

			return carsfFromDb;
		}

		public async Task<Car> GetCar(string ID)
		{
			var cachedCars = await _cache.GetStringAsync("cars");
			if (!string.IsNullOrEmpty(cachedCars))
			{
				var car = JsonConvert.DeserializeObject<List<Car>>(cachedCars).Where(c => ID.Equals(c.ID.ToString())).FirstOrDefault();
				return car;
			}
			var carFromDb = await _dbContext.Cars.Where(c => c.ID.Equals(ID)).FirstOrDefaultAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};

			var carsFromDb = await _dbContext.Cars.OrderBy(c => c.ID).ToListAsync();
			var serializedData = JsonConvert.SerializeObject(carsFromDb);
			await _cache.SetStringAsync("cars", serializedData, cacheOptions);
			return carFromDb;
		}

		public async Task CreateCar(Car car)
		{
			_dbContext.Cars.Add(car);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("cars");
		}

		public async Task DeleteCar(Car car)
		{
			_dbContext.Cars.Remove(car);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("cars");
		}

		public async Task UpdateCar(Car car)
		{
			_dbContext.Update(car);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("cars");
		}

		public async Task<bool> IsAvailableOnDayInterval(string ID, DateTime startDate, DateTime endDate) {
			if (!await doesCarExists(ID)) throw new Exception("Car does not exist");
			Car car = await GetCar(ID);
			if (car == null) return false;
			var carRents = await _dbContext.CarRents.Where(cr => cr.CarID.Equals(ID)).ToListAsync();
			foreach (var rent in carRents)
			{
				if (startDate >= rent.StartDate && startDate <= rent.EndDate) return false;
				if (endDate >= rent.StartDate && endDate <= rent.EndDate) return false;
			}
			return true;
		}

		public async Task<IEnumerable<CarRentDetails>> GetCarRentHistory(string ID)
		{
			if(!await doesCarExists(ID)) throw new Exception("Car does not exist");
			Car car = await GetCar(ID);
			//TODO: ha carrentmanagerservice kesz -> abbol listCarRents
			var carRents = await _dbContext.CarRents.Where(cr => cr.CarID.Equals(ID)).ToListAsync();

			List<CarRentDetails> carRentDetails = new List<CarRentDetails>();
			int i = 0;
			foreach (var carRent in carRents)
			{
				//TODO: ha rentsmanager service kesz -> abbol getRent
				var rent = _dbContext.Rents.Where(r => r.ID.Equals(carRent.RentID)).FirstOrDefault();
				CarRentDetails crd = new CarRentDetails(carRent, rent, i);
				carRentDetails.Add(crd);
				i++;
			}
			return carRentDetails;
		}

		public async Task<bool> doesCarExists(string ID)
		{
			var car = await GetCar(ID);
			return car != null;
		}

	}
}
