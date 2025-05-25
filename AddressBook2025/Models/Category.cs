using AddressBook2025.Client.Models.DTOs;
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

        //mapping Category to CategoryDTO
        public CategoryDTO ToDTO()
        {
            CategoryDTO dto = new()
            {
                Id = this.Id,
                Name = this.Name,
            };
            foreach (Contact contact in Contacts)
            {
                //prevent circular reference
                contact.Categories.Clear();
                dto.Contacts.Add(contact.ToDTO());
            }
            return dto;
        }
    }
}