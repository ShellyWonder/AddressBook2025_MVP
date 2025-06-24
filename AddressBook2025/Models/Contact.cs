using AddressBook2025.Data;
using AddressBook2025.Client.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AddressBook2025.Client.Models.DTOs;
using AddressBook2025.Helpers;

namespace AddressBook2025.Models
{
    public class Contact
    {
        private DateTimeOffset? _created = DateTimeOffset.UtcNow;
        

        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        public DateOnly? BirthDate { get; set; }
       
        [Required]
        [Display(Name = "Address")]
        public string? Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string? Address2 { get; set; }

        [Required]
        public string? City { get; set; }

        [Required]
        public State State { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        [RegularExpression(@"^(?!0{5}|[0-9]{9})[0-9]{5}(?:-[0-9]{4})?$",
         ErrorMessage = "Postal code must be 5 or 9 digits and must not contain all zeros. If 9 digits, it must not begin with 5 zeros or end in 4 zeros.")]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

       
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTimeOffset Created
        {
            get => (DateTimeOffset)_created!;
            set => _created = value.ToUniversalTime();
        }

        [Required]
        public string? AppUserId { get; set; }

        //Creates route; Foreign key to the ApplicationUser table
        public virtual ApplicationUser? AppUser { get; set; }

        public Guid? ImageId { get; set; }
        public virtual ImageUpload? Image { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = [];

        public ContactDTO ToDTO()
        {
            ContactDTO dto = new()
            {
                Id = this.Id,
                FirstName = this.FirstName,
                LastName = this.LastName,
                BirthDate = this.BirthDate.HasValue ? this.BirthDate.Value : default, // Fix for CS8629
                Address1 = this.Address1,
                Address2 = this.Address2,
                City = this.City,
                State = this.State,
                ZipCode = this.ZipCode,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                Created = this.Created.ToLocalTime(),
                ProfileImageUrl = this.ImageId.HasValue ? $"api/uploads/{ImageId}" : ImageHelper.DefaultProfilePictureUrl
            };
            foreach (Category category in Categories)
            {
                // Prevent circular reference
                category.Contacts.Clear();
                dto.Categories.Add(category.ToDTO());
            }

            return dto;
        }
    }
}
