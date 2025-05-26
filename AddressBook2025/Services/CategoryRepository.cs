using Microsoft.EntityFrameworkCore;
using AddressBook2025.Data;

using AddressBook2025.Models;
using AddressBook2025.Services.Interfaces;

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
    }

}
