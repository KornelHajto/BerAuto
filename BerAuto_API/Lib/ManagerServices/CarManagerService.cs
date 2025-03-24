using Microsoft.Extensions.Caching.Distributed;

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

		public void ListCars() 
		{ 
		
		}

		public void GetCar()
		{

		}

		public void CreateCar()
		{

		}

		public void DeleteCar()
		{

		}

		public void UpdateCar()
		{

		}

		public void IsAvailableOnDayInterval() {
		
		}

		public void GetCarRentHistory()
		{

		}


	}
}
