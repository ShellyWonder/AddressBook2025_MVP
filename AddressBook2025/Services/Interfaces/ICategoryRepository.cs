using AddressBook2025.Models;

namespace AddressBook2025.Services.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategoryAsync(Category category);
        Task <List<Category>> GetCategoriesAsync(string userId);
        Task<Category?> GetCategoryByIdAsync(int id, string userId);
        Task UpdateCategoryAsync(Category category, string userId); 

        Task DeleteCategoryAsync(int id, string userId);

    }
}
