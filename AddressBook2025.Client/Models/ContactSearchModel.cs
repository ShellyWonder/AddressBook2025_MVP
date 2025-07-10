using System.ComponentModel.DataAnnotations;

namespace AddressBook2025.Client.Models
{
    public class ContactSearchModel
    {
        [Required(ErrorMessage = "Please enter a name to search.")]
        [MinLength(2, ErrorMessage = "Search term must be at least 2 characters.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "Please use letters, spaces, apostrophes, or hyphens only.")]

        public string? SearchTerm { get; set; }

    }
}
