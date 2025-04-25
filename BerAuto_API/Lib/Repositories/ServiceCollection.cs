using BerAuto.Lib.ManagerServices;
using BerAuto.Lib.Repositories;
using BerAuto_API.Lib.ManagerServices.Interfaces;
using BerAuto_API.Lib.Repositories.Interfaces;

namespace BerAuto_API.Lib.Repositories
{
    public static class ServiceCollection
	{
		public static void AddLocalServices(this IServiceCollection services)
		{
			services.AddScoped<ICarManagerService, CarManagerService>();
			services.AddScoped<ICategoryManagerService,CategoryManagerService>();
			services.AddScoped<IRentalManagerService, RentalManagerService>();
			services.AddScoped<IUnitOfWork, ProductionUnitOfWork>();
			services.AddScoped<IAuthManagerService, AuthManagerService>();

		}
	}
}
