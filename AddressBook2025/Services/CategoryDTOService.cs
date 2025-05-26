using AddressBook2025.Client.Models.DTOs;
using AddressBook2025.Client.Services.Interfaces;
using AddressBook2025.Models;
using AddressBook2025.Services.Interfaces;

namespace AddressBook2025.Services
{
    public class CategoryDTOService(ICategoryRepository repository) : ICategoryDTOService
    {


        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category, string userId)
        {
            Category newCategory = new()
             {
                AppUserId = userId,
                Name = category.Name,
             };
        newCategory = await repository.CreateCategoryAsync(newCategory);
            return newCategory.ToDTO();
        }
        // List of entities and transform them to DTOs
        public async Task<List<CategoryDTO>> GetCategoriesAsync(string userId)
        {
            List<Category> categories = await repository.GetCategoriesAsync(userId);
            return categories.Select(c => c.ToDTO()).ToList();
        }
    }

}
