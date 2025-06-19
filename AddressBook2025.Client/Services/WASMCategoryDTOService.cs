using AddressBook2025.Client.Models;
using AddressBook2025.Client.Models.DTOs;
using AddressBook2025.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace AddressBook2025.Client.Services
{
    public class WASMCategoryDTOService(HttpClient http) : ICategoryDTOService
    {
        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category, string userId)
        {
            HttpResponseMessage response = await http.PostAsJsonAsync("api/categories", category);
            response.EnsureSuccessStatusCode();

            CategoryDTO? createdCategory = await response.Content.ReadFromJsonAsync<CategoryDTO>();
            return createdCategory ?? throw new HttpRequestException("Failed to create category.");
        }

        public async Task<List<CategoryDTO>> GetCategoriesAsync(string userId)
        {
            return await http.GetFromJsonAsync<List<CategoryDTO>>("api/categories") ?? [];
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id, string userId)

        {
            try
            {
                return await http.GetFromJsonAsync<CategoryDTO?>($"api/categories/{id}");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return null; // Return null if the category is not found or an error occurs
            }
        }

        public async Task UpdateCategoryAsync(CategoryDTO category, string userId)
        {
            HttpResponseMessage response = await http.PutAsJsonAsync($"api/categories/{category.Id}", category);
            response.EnsureSuccessStatusCode();

        }

        public async Task DeleteCategoryAsync(int id, string userId)
        {
            HttpResponseMessage response = await http.DeleteAsync($"api/categories/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> EmailCategoryAsync(int id, EmailData emailData, string userId)
        {
            try
            {
                HttpResponseMessage response = await http.PostAsJsonAsync($"api/categories/{id}/email", emailData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                // Return false if an error occurs while sending the email
                return false;
            }
        }
    }
}
