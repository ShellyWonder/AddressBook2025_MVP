using AddressBook2025.Client.Models;
using AddressBook2025.Client.Models.DTOs;
using AddressBook2025.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace AddressBook2025.Client.Services
{
    public class WASMContactDTOService(HttpClient http) : IContactDTOService
    {
        public Task<ContactDTO> CreateContactAsync(ContactDTO contact, string userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteContactAsync(int id, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EmailContactAsync(int id, EmailData emailData, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ContactDTO> GetContactByIdAsync(int id, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ContactDTO>> GetContactsAsync(string userId)
        {
            return await http.GetFromJsonAsync<List<ContactDTO>>($"api/Contacts") ?? [];

        }

        public async Task<List<ContactDTO>> GetContactsByCategoryAsync(int categoryId, string userId)
        {
           return await http.GetFromJsonAsync<List<ContactDTO>>($"api/Contacts?categoryID={categoryId}") ?? [];

        }

        public async Task<List<ContactDTO>> SearchContactsAsync(string searchTerm, string userId)
        {
            try
            {
                return await http.GetFromJsonAsync<List<ContactDTO>>($"api/contacts/search?query={searchTerm}") ?? [];
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public Task UpdateContactAsync(ContactDTO contact, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
