


namespace BerAuto_API.Lib.Repositories.Interfaces
{
	public interface ICategoryRepository
	{
		Task CreateCategory(Category c);
		Task<object?> GetCategory(string iD);
		Task<object?> ListCategories();
		Task<CategoryViewDTO> UpdateCategoryName(string iD, string newName);
		Task<CategoryViewDTO> UpdateCategoryRate(string iD, int newRate);
	}
}