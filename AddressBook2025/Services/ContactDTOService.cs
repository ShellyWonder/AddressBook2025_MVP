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
            //transform entity to DTO
            return newContact.ToDTO();
        }
    }
}
