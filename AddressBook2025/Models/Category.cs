using AddressBook2025.Data;
using System.ComponentModel.DataAnnotations;

namespace AddressBook2025.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string? Name { get; set; }

        [Required]
        public string? AppUserId { get; set; }
        //Creates route; Foreign key to the ApplicationUser table
        public virtual ApplicationUser? AppUser { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; } = [];
    }
}