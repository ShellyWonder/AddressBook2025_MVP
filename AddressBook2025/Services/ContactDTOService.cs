using AddressBook2025.Client.Models.DTOs;
using AddressBook2025.Client.Services.Interfaces;
using AddressBook2025.Helpers;
using AddressBook2025.Models;
using AddressBook2025.Services.Interfaces;

namespace AddressBook2025.Services
{
    public class ContactDTOService(IContactRepository repository) : IContactDTOService
    {
        public async Task<ContactDTO> CreateContactAsync(ContactDTO dto, string userId)
        {
            //transform DTO to entity
            Contact newContact = new()
            {
                AppUserId = userId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address1 = dto.Address1,
                Address2 = dto.Address2,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                Created = DateTimeOffset.UtcNow

            };
            //save image--convert the url to the imageUpload type
            if (dto.ProfileImageUrl?.StartsWith("data:") == true)
            {
                newContact.Image = ImageHelper.GetImageUploadFromURL(dto.ProfileImageUrl);
            }

            //save new object
            newContact = await repository.CreateContactAsync(newContact);

            //add (update new contact)categories to the contact
            List<int> categoryIds = dto.Categories?.Select(c => c.Id).ToList() ?? new List<int>();
            await repository.AddCategoriesToContactAsync(newContact.Id, userId, categoryIds);

            //Since the new contact has been updated with categories, requery the database 
            // read method
            newContact = await repository.GetContactByIdAsync(newContact.Id, userId) 
                                          ?? throw new Exception("Contact not found after creation.");
            //transform entity to DTO
            return newContact.ToDTO();
        }

        public  async Task DeleteContactAsync(int id, string userId)
        {
             await repository.DeleteContactAsync(id, userId);
        }

        public async Task<ContactDTO> GetContactByIdAsync(int id, string userId)
        {
            Contact? contact = await repository.GetContactByIdAsync(id, userId);
            return contact!.ToDTO();

        }

        public async Task<List<ContactDTO>> GetContactsAsync(string userId)
        {
            List<Contact> contacts = await repository.GetContactsAsync(userId);
            //transform entities to DTOs
            List<ContactDTO> dtos = [.. contacts.Select(c=> c.ToDTO())];
            return dtos;
        }

        public async Task<List<ContactDTO>> GetContactsByCategoryAsync(int categoryId, string userId)
        {
            List<Contact> contacts = await repository.GetContactsByCategoryAsync(categoryId, userId);
            List<ContactDTO> dtos = [.. contacts.Select(c => c.ToDTO())];
            return dtos;
        }

        public async Task <List<ContactDTO>> SearchContactsAsync(string searchTerm, string userId)
        {
            List<Contact> contacts = await repository.SearchContactAsync(searchTerm, userId);
            //transform entities to DTOs
            List<ContactDTO> dtos = [.. contacts.Select(c => c.ToDTO())];
            return dtos;
        }

        public async Task UpdateContactAsync(ContactDTO dto, string userId)
        {
            Contact? contact = await repository.GetContactByIdAsync(dto.Id, userId);

            if (contact != null)
            {
                ///<summary>
                //properties that are not included because they do not change on update:
                // Created, AppUserId, and Id
                ///</summary>
                contact.FirstName = dto.FirstName;
                contact.LastName = dto.LastName;
                contact.BirthDate = dto.BirthDate;
                contact.Email = dto.Email;
                contact.PhoneNumber = dto.PhoneNumber;
                contact.Address1 = dto.Address1;
                contact.Address2 = dto.Address2;
                contact.City = dto.City;
                contact.State = dto.State;
                contact.ZipCode = dto.ZipCode;
            }
            //triggers when a user uploads an image during update
            if (dto.ProfileImageUrl?.StartsWith("data:") == true)
            {
                contact.Image = ImageHelper.GetImageUploadFromURL(dto.ProfileImageUrl);
            }
            else
            {
                //user did not select a new image during update
                contact.Image = null;
            }
            //remove categories from the record
            contact.Categories.Clear();
            //save contact object without categories
            await repository.UpdateContactAsync(contact);
            //now remove categories from the join table
            await repository.RemoveCategoriesFromContactAsync(contact.Id, userId);

            //user's updated categories selected from the update(edit) form
            List<int> categoryIds = dto.Categories.Select(c => c.Id).ToList();
            //finally, add categories back to the db; "save" done in repository layer not the dto;
             await repository.AddCategoriesToContactAsync(contact.Id, userId, categoryIds);
        }
    }
}
