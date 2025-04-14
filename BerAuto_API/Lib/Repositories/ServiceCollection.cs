using BerAuto.Lib.ManagerServices;
using BerAuto.Lib.Repositories;
using BerAuto_API.Lib.ManagerServices.Interfaces;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace CryptoSim_API.Lib.UnitOfWork
{
	public static class ServiceCollection
	{
		public static void AddLocalServices(this IServiceCollection services)
		{
			services.AddScoped<ICarManagerService, CarManagerService>();
			services.AddScoped<ICategoryManagerService,CategoryManagerService>();
			services.AddScoped<IRentalManagerService, RentalManagerService>();
			services.AddScoped<IUnitOfWork, ProductionUnitOfWork>();
		}
	}
}
