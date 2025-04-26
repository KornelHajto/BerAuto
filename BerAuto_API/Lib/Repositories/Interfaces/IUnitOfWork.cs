using System;
using System.Threading.Tasks;
using BerAuto.Models;

namespace BerAuto_API.Lib.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IRentalRepository rentalRepository { get; }
        ICategoryRepository categoryRepository { get; }
        ICarRepository carRepository { get; }
        IAuthRepository AuthRepository { get; }
		IUserRepository userRepository { get; }

	}
}
