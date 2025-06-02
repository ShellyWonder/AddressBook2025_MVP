using AddressBook2025.Services.Interfaces;
using AddressBook2025.Models;
using Microsoft.EntityFrameworkCore;
using AddressBook2025.Data;

namespace AddressBook2025.Services
{
    public class ContactRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : IContactRepository
    {
        public async Task<List<Contact>> GetContactsAsync(string userId)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            //read all contacts for the userId
            List<Contact> contacts= await context.Contacts.Include(c => c.Categories) // Include categories if needed
                .Where(c => c.AppUserId == userId)
                .ToListAsync();
            return contacts;
        }
        public async Task<Contact?> GetContactByIdAsync(int Id, string userId)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            //read contact by id and userId
            Contact? contact = await context.Contacts
                .Include(c => c.Categories) // Include categories if needed
                .FirstOrDefaultAsync(c => c.Id == Id && c.AppUserId == userId);

            return contact;
        }

        public async Task<Contact> CreateContactAsync(Contact contact)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            //add contact to the database
            context.Contacts.Add(contact);
            //save changes to the database
            await context.SaveChangesAsync();
            return contact;
        }
      
        //one(contact) to many(category) relationship handled by a db join table
        public async Task AddCategoriesToContactAsync(int contactId, string userId, List<int> categoryIds)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            Contact? contact = await context.Contacts
                .Include(c => c.Categories) // Include existing categories if any;
                .FirstOrDefaultAsync(c => c.Id == contactId && c.AppUserId== userId);
            if (contact is not null)
            {
                //One-to-many relationship: add categories to the contact
                //writes to the join table
                foreach (int categoryId in categoryIds)
                {
                    Category? category = await context.Categories.Include(c => c.Contacts).FirstOrDefaultAsync(c => c.Id == categoryId && c.AppUserId == userId); 

                    if(category is not null) contact.Categories.Add(category);
                    else throw new Exception($"Category with ID {categoryId} not found for user {userId}.");
                }
                //save changes to the database
                await context.SaveChangesAsync();
            }
        }

    }
}
                                                     