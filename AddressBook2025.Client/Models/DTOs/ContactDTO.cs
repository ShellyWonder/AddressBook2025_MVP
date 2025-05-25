using AddressBook2025.Client.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBook2025.Client.Models.DTOs
{
    public class ContactDTO
    {
        private DateTimeOffset? _created;
        private DateTimeOffset? _birthDate;

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
        public DateTimeOffset BirthDate
        {
            get => (DateTimeOffset)_birthDate!;
            set => _birthDate = value.ToUniversalTime();
        }
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
        [StringLength(10)]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Please enter a 5 digit or 9 digit US Postal Code")]
        [DataType(DataType.PostalCode)]
        public int ZipCode { get; set; }

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

        public string? ProfileImageUrl { get; set; }
      public virtual ICollection<CategoryDTO> Categories { get; set; } = [];
    }
}

