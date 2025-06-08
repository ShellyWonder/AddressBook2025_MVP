using AddressBook2025.Services.Interfaces;
using AddressBook2025.Models;
using AddressBook2025.Helpers;
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
            List<Contact> contacts = await context.Contacts.Include(c => c.Categories) // Include categories if needed
                .Where(ContactPredicates.ByUserId(userId))
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
                .FirstOrDefaultAsync(ContactPredicates.ByIdAndUser(Id, userId));

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
                .FirstOrDefaultAsync(ContactPredicates.ByIdAndUser(contactId, userId));
            if (contact is not null)
            {
                //One-to-many relationship: add categories to the contact
                //writes to the join table
                foreach (int categoryId in categoryIds)
                {
                    Category? category = await context.Categories.Include(c => c.Contacts).FirstOrDefaultAsync(CategoryPredicates.ByCategoryIdAndUser(categoryId, userId));

                    if (category is not null) contact.Categories.Add(category);
                    else throw new Exception($"Category with ID {categoryId} not found for user {userId}.");
                }
                //save changes to the database
                await context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Just updating Contact fields in this method;
        /// Categories are updated in the DTO Service layer due to category's many to many relationship
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task UpdateContactAsync(Contact contact)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            if (await context.Contacts.AnyAsync(ContactPredicates.ByContactIdAndContactUser(contact)))
            {
                //If user does not browse for a new image, the old image is left alone and remains null
                ImageUpload? OldImage = null;

                //triggers when user browses for new image
                if (contact.Image is not null)
                {
                    //look for the old image
                    if (contact.ImageId != contact.ImageId) OldImage = await context.Images.FirstOrDefaultAsync(img => img.Id == contact.ImageId);
                    //save the new image-- overriding the oldImage id
                    //save the child first
                    if (contact.Image?.Id != null) contact.ImageId = contact.Image.Id;
                    context.Images.Add(contact.Image!);
                }

                context.Contacts.Update(contact);
                await context.SaveChangesAsync();
                if (OldImage is not null)
                {
                    context.Images.Remove(OldImage);
                    await context.SaveChangesAsync();

                }
            }
        }

        public async Task RemoveCategoriesFromContactAsync(int contactId, string userId)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            Contact? contact = await context.Contacts
                                           .Include(c => c.Categories)
                                           .FirstOrDefaultAsync(ContactPredicates.ByContactIdAndUser(contactId, userId));

            if (contact is not null)
            {
                //remove existing categories from the join table
                contact.Categories.Clear();
                //save changes
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteContactAsync(int id, string userId)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            Contact? contact = await context.Contacts.FirstOrDefaultAsync(ContactPredicates.ByIdAndUser(id, userId));
            if (contact is not null)
            {
                context.Contacts.Remove(contact);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Contact>> SearchContactAsync(string searchTerm, string userId)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            //convert all alpha characters to lowercase and trim spaces and non alpha characters
            string searchTermLower = searchTerm.Trim().ToLower();
            List<Contact> contacts = await context.Contacts.Where(ContactPredicates.ByUserId(userId))
                                                           .Include(c => c.Categories)
                                                           .Where(c => string.IsNullOrEmpty(searchTermLower)
                                                            || c.FirstName!.ToLower().Contains(searchTermLower)
                                                            || c.LastName!.ToLower().Contains(searchTermLower)
                                                            || c.Categories.Any(cat => cat.Name!.ToLower()
                                                                 .Contains(searchTermLower))

                                                           ).ToListAsync();
            return contacts;

        }

        public  async Task<List<Contact>> GetContactsByCategoryAsync(int categoryId, string userId)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();
                Category? category = await context.Categories.Include(c => c.Contacts)
                                                            .ThenInclude(c => c.Categories)
                                                            .FirstOrDefaultAsync(CategoryPredicates.ByCategoryIdAndUser(categoryId,userId));

            //returns ONE category with list of contacts : returns an empty array;
            return category?.Contacts.ToList() ?? [];
        }
    }
}
