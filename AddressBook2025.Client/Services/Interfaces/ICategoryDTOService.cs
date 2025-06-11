using AddressBook2025.Client.Models;
using AddressBook2025.Client.Models.DTOs;

namespace AddressBook2025.Client.Services.Interfaces
{
    public interface ICategoryDTOService
    {
        //create a new category
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category, string userId);

        //get (read) all categories for a user
        Task<List<CategoryDTO>> GetCategoriesAsync(string userId);

        Task<CategoryDTO> GetCategoryByIdAsync(int id, string userId);

        //update a category
        Task UpdateCategoryAsync(CategoryDTO category, string userId);

        //delete a category 
        Task DeleteCategoryAsync(int id, string userId);

        //send email
        Task <bool> EmailCategoryAsync(int id, EmailData emailData, string userId);
    }
}
