using AddressBook2025.Client.Models;
using AddressBook2025.Client.Models.DTOs;

namespace AddressBook2025.Client.Services.Interfaces
{
    public interface IContactDTOService
    {
        //create a contact
        Task<ContactDTO> CreateContactAsync(ContactDTO contact, string userId);

        //read a contact
        Task <List<ContactDTO>>GetContactsAsync(string userId);

        Task<ContactDTO?>GetContactByIdAsync(int id, string userId);

        //update
        Task UpdateContactAsync(ContactDTO contact, string userId);

        //delete 
        Task DeleteContactAsync(int id, string userId);
        //Search
        Task<List<ContactDTO>> SearchContactsAsync( string searchTerm,string userId);

        //filter by category
        Task<List<ContactDTO>> GetContactsByCategoryAsync(int categoryId, string userId);

        //email
        Task<bool> EmailContactAsync(int id, EmailData emailData, string userId);
    }
}
