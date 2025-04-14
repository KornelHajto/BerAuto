using System;
using System.Threading.Tasks;
using BerAuto.Models;

namespace BerAuto.Lib.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Car> Cars { get; }
        IRepository<CarRent> CarRents { get; }
        IRepository<Category> Categories { get; }
        IRepository<Log> Logs { get; }
        IRepository<Rent> Rents { get; }
        IRepository<User> Users { get; }
        API_DbContext DbContext { get; }
        Task<int> CompleteAsync();
    }
}
