

namespace BerAuto_API.Lib.ManagerServices.Interfaces
{
	public interface ICategoryManagerService
	{
		Task CreateCategory(Category c);
		Task<CategoryViewDTO> GetCategory(string iD);
		Task<IEnumerable<CategoryViewDTO>> ListCategories();
		Task<CategoryViewDTO> UpdateCategoryName(string iD, string newName);
		Task<CategoryViewDTO> UpdateCategoryRate(string iD, int newRate);
	}
}