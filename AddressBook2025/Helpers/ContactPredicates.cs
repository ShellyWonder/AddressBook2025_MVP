using AddressBook2025.Models;
using System.Linq.Expressions;

namespace AddressBook2025.Helpers
{
    public static class ContactPredicates
    {
        public static Expression<Func<Contact, bool>> ByUserId(string userId) =>
            c => c.AppUserId == userId;

        //Matches the single contact that belongs to a logged in user
        public static Expression<Func<Contact, bool>> ByIdAndUser(int id, string userId) =>
            c=> c.Id == id && c.AppUserId == userId;

        public static Expression<Func<Contact, bool>> ByContactIdAndContactUser(Contact contact) =>
            c => c.Id == contact.Id && c.AppUserId == contact.AppUserId;
        public static Expression<Func<Contact, bool>> ByContactIdAndUser(int contactId, string userId) =>
            c => c.Id == contactId && c.AppUserId == userId;

        
    }
}
