using BerAuto.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Linq;

namespace BerAuto.Lib.ManagerServices
{
	public class CarManagerService
	{
		private readonly API_DbContext _dbContext;
		private readonly IDistributedCache _cache;

		CategoryManagerService categoryManager;

		public CarManagerService(API_DbContext dbContext, IDistributedCache cache)
		{
			_dbContext = dbContext;
			_cache = cache;

			categoryManager = new CategoryManagerService(dbContext, cache);
		}

		public async Task<IEnumerable<CarViewDTO>> ListCars() 
		{
			var cars = await getCarsCache();
			if(cars == null)
			{
				cars = await getCarsDB();
			}
			if(cars == null)
			{
				return null;
			}
			return await convertListToDTO(cars);
		}

		public async Task<CarViewDTO> GetCarDTO(string ID)
		{
			var exists = await doesCarExists(ID);
			if (exists)
			{
				var cars = await getCarsCache();
				if (cars == null)
				{
					cars = await getCarsDB();
				}
				var car = cars.Where(c => ID.Equals(c.ID.ToString())).FirstOrDefault();
				return await convertCarToCarViewDTO(car.ID);
			}
			return null;
		}

		public async Task CreateCar(Car car)
		{
			car.Description = $"{DateTime.Now.ToString()} : {car.Description}";
			_dbContext.Cars.Add(car);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("cars");
		}

		public async Task DeleteCar(string ID)
		{
			var car = await GetCar(ID);
			_dbContext.Cars.Remove(car);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("cars");
		}

		public async Task<CarViewDTO> UpdateCarCategory(string carId, string categoryId)
		{
			bool exists = await doesCarExists(carId) && await categoryManager.doesCategoryExists(categoryId);
			if (!exists) throw new Exception("Car or Category does not exist");
			var car = await GetCar(carId);
			car.CategoryId = Guid.Parse(categoryId);
			await UpdateCar(car);
			return await convertCarToCarViewDTO(car.ID);
		}

		public async Task<CarViewDTO> UpdateCarOdometer(string carId, int newKm)
		{
			if (!await doesCarExists(carId)) throw new Exception("Car does not exist");
			var car = await GetCar(carId);
			car.Odometer = newKm;
			await UpdateCar(car);
			return await convertCarToCarViewDTO(car.ID);
		}
		
		public async Task<CarViewDTO> UpdateCarAvailablity(string carId, bool available)
		{
			if (!await doesCarExists(carId)) throw new Exception("Car does not exist");
			var car = await GetCar(carId);
			car.Available = available;
			await UpdateCar(car);
			return await convertCarToCarViewDTO(car.ID);
		}

		public async Task<CarViewDTO> AppendCarDescription(string carId, string newDesc)
		{
			if (!await doesCarExists(carId)) throw new Exception("Car does not exist");
			var car = await GetCar(carId);
			car.Description.Concat("\n");
			car.Description.Concat($"{DateTime.Now.ToString()} : {newDesc}");
			await UpdateCar(car);
			return await convertCarToCarViewDTO(car.ID);
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

		public async Task<IEnumerable<CarRentViewDTO>> GetCarRentHistory(string ID)
		{
			if(!await doesCarExists(ID)) throw new Exception("Car does not exist");
			Car car = await GetCar(ID);
			//TODO: ha carrentmanagerservice kesz -> abbol listCarRents
			var carRents = await _dbContext.CarRents.Where(cr => cr.CarID.Equals(ID)).ToListAsync();

			List<CarRentViewDTO> carRentDetails = new List<CarRentViewDTO>();
			int i = 0;
			foreach (var carRent in carRents)
			{
				//TODO: ha rentsmanager service kesz -> abbol getRent
				var rent = _dbContext.Rents.Where(r => r.ID.Equals(carRent.RentID)).FirstOrDefault();
				CarRentViewDTO crd = new CarRentViewDTO(carRent, rent, i);
				carRentDetails.Add(crd);
				i++;
			}
			return carRentDetails;
		}

		public async Task<IEnumerable<CarViewDTO>> listCarsWithCategory(string ID)
		{
			var cars = await getCarsCache();
			if (cars == null)
			{
				cars = await getCarsDB();
			}
			cars = cars.Where(c => c.CategoryId.ToString().Equals(ID)).AsQueryable();
			if (cars == null)
			{
				return null;
			}
			return await convertListToDTO(cars);
		}

		

		//Private methods:
		private async Task<IQueryable<Car>> getCarsCache()
		{
			var cachedCars = await _cache.GetStringAsync("cars");
			if (!string.IsNullOrEmpty(cachedCars))
			{
				var cars = JsonConvert.DeserializeObject<List<Car>>(cachedCars);
				return cars.AsQueryable<Car>();
			}
			return null;
		}

		private async Task<IQueryable<Car>> getCarsDB()
		{
			var carsfFromDb = await _dbContext.Cars.OrderBy(c => c.ID).ToListAsync();
			var cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};
			var serializedData = JsonConvert.SerializeObject(carsfFromDb);
			await _cache.SetStringAsync("cars", serializedData, cacheOptions);
			return _dbContext.Cars.OrderBy(c => c.ID).Include(c => c.Category);
		}

		private async Task<IEnumerable<CarViewDTO>> convertListToDTO(IQueryable<Car> cars)
		{
			foreach (var car in cars)
			{
				car.Category = await categoryManager.GetCategory(car.CategoryId.ToString());
			}
			IEnumerable<CarViewDTO> response = cars.Include(c => c.Category).Select(c => new CarViewDTO
			{
				ID = c.ID,
				PlateNumber = c.PlateNumber,
				Type = c.Type,
				Odometer = c.Odometer,
				Available = c.Available,
				Description = c.Description,
				CategoryName = c.Category.Name,
				DailyRate = c.Category.DailyRate
			}).ToList();
			return response;
		}
		private async Task<bool> doesCarExists(string ID)
		{
			var car = await GetCar(ID);
			return car != null;
		}

		private async Task<Car> GetCar(string ID)
		{
			var cars = await getCarsCache();
			if (cars == null)
			{
				cars = await getCarsDB();
			}
			var car = cars.Where(c => ID.Equals(c.ID.ToString())).FirstOrDefault();
			return car;
		}

		private async Task UpdateCar(Car car)
		{
			_dbContext.Update(car);
			await _dbContext.SaveChangesAsync();
			await _cache.RemoveAsync("cars");
		}

		private async Task<CarViewDTO> convertCarToCarViewDTO(Guid ID) {
			var car = await GetCar(ID.ToString());
			var category = await categoryManager.GetCategory(car.CategoryId.ToString());
			return new CarViewDTO
			{
				ID = car.ID,
				PlateNumber = car.PlateNumber,
				Type = car.Type,
				Odometer = car.Odometer,
				Available = car.Available,
				Description = car.Description,
				CategoryName = category.Name,
				DailyRate = category.DailyRate
			};
		}
	}
}
