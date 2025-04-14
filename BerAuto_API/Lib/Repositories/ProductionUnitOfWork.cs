using System.Threading.Tasks;
using BerAuto.Lib.Database;
using BerAuto.Models;
using BerAuto_API.Lib.Repositories.Interfaces;

namespace BerAuto.Lib.Repositories
{
    public class ProductionUnitOfWork(IServiceScopeFactory scopeFactory) : IUnitOfWork
    {
		public IRentalRepository rentalRepository { get; private set; } = new RentalRepository(scopeFactory);
		public ICategoryRepository categoryRepository { get; private set; } = new CategoryRepository(scopeFactory);
		public ICarRepository carRepository { get; private set; } = new CarRepository(scopeFactory);

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
