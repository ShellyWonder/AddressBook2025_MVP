using AddressBook2025.Client.Models;
using AddressBook2025.Client.Models.DTOs;
using AddressBook2025.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace AddressBook2025.Client.Services
{
    public class WASMContactDTOService(HttpClient http) : IContactDTOService
    {
        #region CREATE METHOD
        public async Task<ContactDTO> CreateContactAsync(ContactDTO contact, string userId)
        {
            HttpResponseMessage response = await http.PostAsJsonAsync("api/contacts", contact);
            response.EnsureSuccessStatusCode();
            ContactDTO? createdContact = await response.Content.ReadFromJsonAsync<ContactDTO?>();

            return createdContact ?? throw new HttpRequestException("Invalid JSON response from server");
        }
        #endregion

        #region DELETE METHOD
        public async Task DeleteContactAsync(int id, string userId)
        {
            HttpResponseMessage response = await http.DeleteAsync($"api/contacts/{id}");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        public Task<bool> EmailContactAsync(int id, EmailData emailData, string userId)
        {
            throw new NotImplementedException();
        }
        #region GET METHODS
        public async Task<ContactDTO?> GetContactByIdAsync(int id, string userId)
        {
            try
            {
                return await http.GetFromJsonAsync<ContactDTO>($"api/contacts/{id}");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<List<ContactDTO>> GetContactsAsync(string userId)
        {
            return await http.GetFromJsonAsync<List<ContactDTO>>($"api/Contacts") ?? [];

        }

        public async Task<List<ContactDTO>> GetContactsByCategoryAsync(int categoryId, string userId)
        {
            return await http.GetFromJsonAsync<List<ContactDTO>>($"api/Contacts?categoryID={categoryId}") ?? [];

        }

        #region SEARCH
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
        #endregion
        #endregion

        #region UPDATE(EDIT) METHOD
        public async Task UpdateContactAsync(ContactDTO contact, string userId)
        {
            HttpResponseMessage response = await http.PutAsJsonAsync($"api/contacts/{contact.Id}", contact);
            response.EnsureSuccessStatusCode();

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"PUT /contacts returned {(int)response.StatusCode}: {body}");
            }
        }
        #endregion
    }
}
