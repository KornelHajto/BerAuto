using BerAuto.DTO;
using BerAuto.Models;
using BerAuto_API.Lib.ManagerServices.Interfaces;
using BerAuto_Lib.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BerAuto.Lib.ManagerServices
{
    public class RentalManagerService :IRentalManagerService
    {
        private readonly API_DbContext _dbContext;
        private readonly IDistributedCache _cache;
        private readonly CarManagerService _carManager; //switch for servicefactory

        public RentalManagerService(API_DbContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
            _carManager = new CarManagerService(dbContext, cache);
        }

        public async Task<IEnumerable<RentViewDTO>> ListRentals()
        {
            var rentals = await getRentalsCache();
            if (rentals == null)
            {
                rentals = await getRentalsDB();
            }
            if (rentals == null)
            {
                return null;
            }
            return await convertListToDTO(rentals);
        }

        public async Task<RentViewDTO> GetRentalDTO(string ID)
        {
            var exists = await doesRentalExist(ID);
            if (exists)
            {
                var rentals = await getRentalsCache();
                if (rentals == null)
                {
                    rentals = await getRentalsDB();
                }
                var rental = rentals.Where(r => ID.Equals(r.ID.ToString())).FirstOrDefault();
                return await convertRentToRentViewDTO(rental.ID);
            }
            return null;
        }

        public async Task<Guid> CreateRental(NewRentDTO newRent)
        {
            Rent rent = new Rent();
			rent.ID = Guid.NewGuid();
			rent.RenterID = Guid.Parse(newRent.RenterID);
			rent.ApplicationTime = DateTime.Now;
			rent.Status = ERentStatus.Request;
			rent.Owed = newRent.Owed;
            Guid id = await AddRent(rent);
            return id;

		}

		private async Task<Guid> AddRent(Rent rent)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				await _dbContext.Rents.AddAsync(rent);
				await _dbContext.SaveChangesAsync();
				await transaction.CommitAsync();
				await _cache.RemoveAsync("rentals");
				return rent.ID;
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error creating rental: " + ex.Message);
			}
			finally
			{
				await transaction.DisposeAsync();
			}
		}

		public async Task<Guid> CreateRentalWithDetails(Guid renterId, Guid carId, DateTime startDate, DateTime endDate)
        {
            var isAvailable = await _carManager.IsAvailableOnDayInterval(carId.ToString(), startDate, endDate);
            if (!isAvailable)
                throw new Exception("Car is not available for the selected period");

            var rental = new Rent
            {
                ID = Guid.NewGuid(),
                RenterID = renterId,
                Status = ERentStatus.Request, // Using the correct enum value
                ApplicationTime = DateTime.Now,
                Owed = 0
            };

            var carRental = new CarRent
            {
                RentID = rental.ID,
                CarID = carId,
                StartDate = startDate,
                EndDate = endDate
            };

			var car = await _dbContext.Cars
				.Include(c => c.Category)
				.FirstOrDefaultAsync(c => c.ID == carId);

			if (car != null)
			{
				int days = Math.Max(1, (int)(endDate - startDate).TotalDays);
				rental.Owed = days * car.Category.DailyRate;
			}

			Guid rentalId = await AddDetailedRent(rental, carRental);

            return rentalId;
        }

		private async Task<Guid> AddDetailedRent(Rent rental, CarRent carRental)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				await _dbContext.Rents.AddAsync(rental);
				await _dbContext.CarRents.AddAsync(carRental);
				await _dbContext.SaveChangesAsync();
				await transaction.CommitAsync();
				await _cache.RemoveAsync("rentals");
				return rental.ID;
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error creating detailed rental: " + ex.Message);
			}
			finally
			{
				await transaction.DisposeAsync();
			}
		}

		public async Task<RentViewDTO> UpdateRentalStatus(string ID, ERentStatus newStatus)
        {
            if (!await doesRentalExist(ID)) throw new Exception("Rental does not exist");

            var rental = await GetRental(ID);
            rental.Status = newStatus;

            // If status changed to InProcess or Returned
            if (newStatus == ERentStatus.InProcess || newStatus == ERentStatus.Returned)
            {
                var carRent = await _dbContext.CarRents
                    .FirstOrDefaultAsync(cr => cr.RentID == rental.ID);

                if (carRent != null)
                {
                    // If returned, make car available again
                    if (newStatus == ERentStatus.Returned)
                    {
                        await _carManager.UpdateCarAvailablity(carRent.CarID.ToString(), true);
                    }
                    else if (newStatus == ERentStatus.InProcess)
                    {
                        // If in process, mark car as unavailable
                        await _carManager.UpdateCarAvailablity(carRent.CarID.ToString(), false);
                    }
                }
            }

            await UpdateRental(rental);
            return await convertRentToRentViewDTO(rental.ID);
        }

        public async Task<CarRentViewDTO> GetRentalDetailsForInvoice(string ID)
        {
            if (!await doesRentalExist(ID)) throw new Exception("Rental does not exist");

            var rental = await GetRental(ID);
            var carRent = await _dbContext.CarRents
                .FirstOrDefaultAsync(cr => cr.RentID == rental.ID);

            if (carRent == null)
                return null;

            return new CarRentViewDTO(carRent, rental, 0);
        }

        public async Task<IEnumerable<RentViewDTO>> SearchRentals(string searchTerm)
        {
            var rentals = await getRentalsCache();
            if (rentals == null)
            {
                rentals = await getRentalsDB();
            }

            searchTerm = searchTerm.ToLower();

            var filteredRentals = rentals.Where(r =>
                r.ID.ToString().ToLower().Contains(searchTerm) ||
                _dbContext.Users.FirstOrDefault(u => u.ID == r.RenterID).Name.ToLower().Contains(searchTerm) == true
            );

            return await convertListToDTO(filteredRentals);
        }

        public async Task<CarViewDTO> GetCarForRental(string rentalId)
        {
            if (!await doesRentalExist(rentalId)) throw new Exception("Rental does not exist");

            var carRent = await _dbContext.CarRents
                .FirstOrDefaultAsync(cr => cr.RentID == Guid.Parse(rentalId));

            if (carRent == null)
                return null;

            return await _carManager.GetCarDTO(carRent.CarID.ToString());
        }

        public async Task<IEnumerable<RentViewDTO>> GetUserRentals(string userId)
        {
            var rentals = await getRentalsCache();
            if (rentals == null)
            {
                rentals = await getRentalsDB();
            }

            var userRentals = rentals.Where(r => r.RenterID.ToString() == userId);
            return await convertListToDTO(userRentals);
        }

        private async Task<IQueryable<Rent>> getRentalsCache()
        {
            var cachedRentals = await _cache.GetStringAsync("rentals");
            if (!string.IsNullOrEmpty(cachedRentals))
            {
                var rentals = JsonConvert.DeserializeObject<List<Rent>>(cachedRentals);
                return rentals.AsQueryable();
            }
            return null;
        }

        private async Task<IQueryable<Rent>> getRentalsDB()
        {
            var rentalsFromDb = await _dbContext.Rents.OrderBy(r => r.ApplicationTime).ToListAsync();
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            var serializedData = JsonConvert.SerializeObject(rentalsFromDb);
            await _cache.SetStringAsync("rentals", serializedData, cacheOptions);
            return _dbContext.Rents.OrderBy(r => r.ApplicationTime);
        }

        private async Task<IEnumerable<RentViewDTO>> convertListToDTO(IQueryable<Rent> rentals)
        {
            List<RentViewDTO> dtoList = new List<RentViewDTO>();

            foreach (var rental in rentals)
            {
                var dto = await convertRentToRentViewDTO(rental.ID);
                if (dto != null)
                {
                    dtoList.Add(dto);
                }
            }

            return dtoList;
        }

        private async Task<bool> doesRentalExist(string ID)
        {
            var rental = await GetRental(ID);
            return rental != null;
        }

        private async Task<Rent> GetRental(string ID)
        {
            var rentals = await getRentalsCache();
            if (rentals == null)
            {
                rentals = await getRentalsDB();
            }

            return rentals.Where(r => ID.Equals(r.ID.ToString())).FirstOrDefault();
        }

        private async Task UpdateRental(Rent rental)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				_dbContext.Rents.Update(rental);
				await _dbContext.SaveChangesAsync();
				await transaction.CommitAsync();
				await _cache.RemoveAsync("rentals");
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error updating rental: " + ex.Message);
			}
			finally
			{
				await transaction.DisposeAsync();
			}
		}

        private async Task<RentViewDTO> convertRentToRentViewDTO(Guid ID)
        {
            var rent = await GetRental(ID.ToString());
            var user = await _dbContext.Users.FindAsync(rent.RenterID);
            return (rent, user?.Name ?? "Unknown").Adapt<RentViewDTO>();
        }

    }
}