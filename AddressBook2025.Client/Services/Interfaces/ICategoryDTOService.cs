using AddressBook2025.Client.Models.DTOs;

namespace AddressBook2025.Client.Services.Interfaces
{
    public interface ICategoryDTOService
    {
        //create a new category
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category, string userId);
    }
}
