using AddressBook2025.Client.Models.DTOs;

namespace AddressBook2025.Client.Services.Interfaces
{
    public interface IContactDTOService
    {
        //create a contact
        Task<ContactDTO> CreateContactAsync(ContactDTO contact, string userId);

        //read a contact
        Task <List<ContactDTO>>GetContactsAsync(string userId);

        Task<ContactDTO>GetContactByIdAsync(int id, string userId);

        //update
        Task UpdateContactAsync(ContactDTO contact, string userId);

    }
}
