
using AddressBook2025.Models;

namespace AddressBook2025.Services.Interfaces
{
    public interface IContactRepository
    {
        Task<Contact> CreateContactAsync(Contact contact);
    }
}
