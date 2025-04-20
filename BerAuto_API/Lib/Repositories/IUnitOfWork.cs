using System;
using System.Threading.Tasks;
using BerAuto.Models;
using BerAuto_API.Lib.Repositories.Interfaces;

namespace BerAuto.Lib.Repositories
{
    public interface IUnitOfWork 
    {
        IRentalRepository rentalRepository{ get; }
		ICategoryRepository categoryRepository { get; }
		ICarRepository carRepository { get; }
	}
}
