using AddressBook2025.Models;
using System.Linq.Expressions;

namespace AddressBook2025.Helpers
{
    public static class CategoryPredicates
    {
        public static Expression<Func<Category, bool>> ByCategoryIdAndUser(int categoryId, string userId) =>
                                          c => c.Id == categoryId && c.AppUserId == userId;
        public static Expression<Func<Category, bool>> ByIdAndUser(Category category, string userId) =>
            c => c.Id == category.Id && c.AppUserId == userId;
 
    }
}
