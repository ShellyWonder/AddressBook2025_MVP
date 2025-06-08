
using AddressBook2025.Models;

namespace AddressBook2025.Services.Interfaces
{
    public interface IContactRepository
    {
        // create a contact first, then add categories to the contact
        Task<Contact> CreateContactAsync(Contact contact);

        //read
        Task<Contact?> GetContactByIdAsync(int Id, string userId);
        Task<List<Contact>> GetContactsAsync(string userId);
        Task<List<Contact>> SearchContactAsync(string searchTerm,string userId);
        Task<List<Contact>> GetContactsByCategoryAsync(int categoryId, string userId);

        //update to add categories to the contact once the contact is created
        Task AddCategoriesToContactAsync(int contactId, string userId, List<int> categoryIds);

        //update
        Task UpdateContactAsync(Contact contact);
        Task RemoveCategoriesFromContactAsync(int contactId, string userId);

        //delete 
        Task DeleteContactAsync(int id, string userId);

    }
}
