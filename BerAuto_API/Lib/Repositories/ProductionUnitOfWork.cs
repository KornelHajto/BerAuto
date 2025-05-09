using System.Threading.Tasks;
using BerAuto.Lib.Database;
using BerAuto.Models;
using BerAuto_API.Lib.Repositories;
using BerAuto_API.Lib.Repositories.Interfaces;


namespace BerAuto.Lib.Repositories
{
    public class ProductionUnitOfWork : IUnitOfWork
    {
		public IRentalRepository rentalRepository { get; } 
		public ICategoryRepository categoryRepository { get;}
		public ICarRepository carRepository { get; }
        public IAuthRepository AuthRepository { get; }
		public IUserRepository userRepository { get; }


		public ProductionUnitOfWork(IServiceScopeFactory scopeFactory)
		{
			rentalRepository = new RentalRepository(scopeFactory);
			categoryRepository = new CategoryRepository(scopeFactory);
			carRepository = new CarRepository(scopeFactory);
            AuthRepository = new AuthRepository(scopeFactory);
			userRepository = new UserRepository(scopeFactory);
		}

    }
}
