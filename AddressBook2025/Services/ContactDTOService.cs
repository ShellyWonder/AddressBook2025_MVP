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
                Created = DateTime.Now

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

        public async Task<List<ContactDTO>> GetContactsAsync(string userId)
        {
            List<Contact> contacts = await repository.GetContactsAsync(userId);
            //transform entities to DTOs
            List<ContactDTO> dtos = [.. contacts.Select(c=> c.ToDTO())];
            return dtos;
        }
    }
}
