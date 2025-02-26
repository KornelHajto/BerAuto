using Microsoft.EntityFrameworkCore;

namespace BerAuto_API.Lib.Migration
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using API_DbContext dbContext = scope.ServiceProvider.GetRequiredService<API_DbContext>();

            dbContext.Database.Migrate();
        }

    }
}
