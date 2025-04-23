using BerAuto_API.Lib.ManagerServices.Interfaces;
using BerAuto_API.Lib.Repositories.Interfaces;

namespace BerAuto_API.Lib.Repositories
{
    internal class CategoryRepository : ICategoryRepository
    {
        public CategoryRepository(IServiceScopeFactory scopeFactory)
        {
            ScopeFactory = scopeFactory;
        }

        public IServiceScopeFactory ScopeFactory { get; }

        public async Task CreateCategory(Category c)
        {
            var scope = ScopeFactory.CreateScope();
            var categoryManager = scope.ServiceProvider.GetRequiredService<ICategoryManagerService>();
            await categoryManager.CreateCategory(c);
        }

        public async Task<object?> GetCategory(string iD)
        {
            var scope = ScopeFactory.CreateScope();
            var categoryManager = scope.ServiceProvider.GetRequiredService<ICategoryManagerService>();
            return await categoryManager.GetCategory(iD);
        }

        public async Task<object?> ListCategories()
        {
            var scope = ScopeFactory.CreateScope();
            var categoryManager = scope.ServiceProvider.GetRequiredService<ICategoryManagerService>();
            return await categoryManager.ListCategories();
        }

        public async Task<CategoryViewDTO> UpdateCategoryName(string iD, string newName)
        {
            var scope = ScopeFactory.CreateScope();
            var categoryManager = scope.ServiceProvider.GetRequiredService<ICategoryManagerService>();
            return await categoryManager.UpdateCategoryName(iD, newName);
        }

        public async Task<CategoryViewDTO> UpdateCategoryRate(string iD, int newRate)
        {
            var scope = ScopeFactory.CreateScope();
            var categoryManager = scope.ServiceProvider.GetRequiredService<ICategoryManagerService>();
            return await categoryManager.UpdateCategoryRate(iD, newRate);
        }
    }
}