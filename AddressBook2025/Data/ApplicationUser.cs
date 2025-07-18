using AddressBook2025.Models;
using Microsoft.AspNetCore.Identity;

namespace AddressBook2025.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //extending IdentityUser with custom properties
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public Guid? ProfilePictureId { get; set; }

        //navigation property within Db for profile picture
        public virtual ImageUpload? ProfilePicture { get; set; } //navigation property for profile picture

        public virtual ICollection<Contact> Contacts { get; set; } = [];
        public virtual ICollection<Category> Categories { get; set; } = [];
    }

}
