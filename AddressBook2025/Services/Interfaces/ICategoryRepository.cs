using AddressBook2025.Models;
using AddressBook2025.Client.Models.DTOs;

namespace AddressBook2025.Services.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategoryAsync(Category category);
    }
}
