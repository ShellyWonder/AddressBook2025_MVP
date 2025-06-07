using Microsoft.EntityFrameworkCore;
using AddressBook2025.Data;

using AddressBook2025.Models;
using AddressBook2025.Services.Interfaces;
using AddressBook2025.Helpers;

namespace AddressBook2025.Services
{
    public class CategoryRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : ICategoryRepository
    {

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            //dbconnection
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            //add category to the database
            context.Categories.Add(category);
            //save changes to the database
            await context.SaveChangesAsync();
            return category;
        }



        public async Task<List<Category>> GetCategoriesAsync(string userId)
        {
            //dbconnection via factory
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            //get all categories for the logged in user
            List<Category> categories = await context.Categories
                .Where(c => c.AppUserId == userId)
                .Include(c => c.Contacts) // Include related contacts
                .ToListAsync();
            return categories;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id, string userId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            //get category by id and userId
            Category? category = await context.Categories
                                      .Include(c => c.Contacts)
                                       .FirstOrDefaultAsync(CategoryPredicates.ByCategoryIdAndUser(id, userId));
            return category;
        }

        public async Task UpdateCategoryAsync(Category category, string userId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            if (await context.Categories.AnyAsync(CategoryPredicates.ByIdAndUser(category, userId)))
            {
                context.Categories.Update(category);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteCategoryAsync(int id, string userId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            Category? category = context.Categories
                .FirstOrDefault(CategoryPredicates.ByCategoryIdAndUser(id, userId));

            if (category is not null)
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }
        }
    }

}
