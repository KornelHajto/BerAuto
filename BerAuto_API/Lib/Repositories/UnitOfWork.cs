using System.Threading.Tasks;
using BerAuto.Lib.Database;
using BerAuto.Models;

namespace BerAuto.Lib.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly API_DbContext _context;

        public IRepository<Car> Cars { get; private set; }
        public IRepository<CarRent> CarRents { get; private set; }
        public IRepository<Category> Categories { get; private set; }
        public IRepository<Log> Logs { get; private set; }
        public IRepository<Rent> Rents { get; private set; }
        public IRepository<User> Users { get; private set; }
        public API_DbContext DbContext => _context;

        public UnitOfWork(API_DbContext context)
        {
            _context = context;
            Cars = new GenericRepository<Car>(context);
            CarRents = new GenericRepository<CarRent>(context);
            Categories = new GenericRepository<Category>(context);
            Logs = new GenericRepository<Log>(context);
            Rents = new GenericRepository<Rent>(context);
            Users = new GenericRepository<User>(context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
