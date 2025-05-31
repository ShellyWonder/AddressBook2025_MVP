using AddressBook2025.Services.Interfaces;
using AddressBook2025.Models;
using Microsoft.EntityFrameworkCore;
using AddressBook2025.Data;

namespace AddressBook2025.Services
{
    public class ContactRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : IContactRepository
    {


        public async Task<Contact> CreateContactAsync(Contact contact)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            //add category to the database
            context.Contacts.Add(contact);
            //save changes to the database
            await context.SaveChangesAsync();
            return contact;
        }

        
    }
}
