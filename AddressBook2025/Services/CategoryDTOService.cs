﻿using AddressBook2025.Client.Models;
using AddressBook2025.Client.Models.DTOs;
using AddressBook2025.Client.Services.Interfaces;
using AddressBook2025.Models;
using AddressBook2025.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AddressBook2025.Services
{
    public class CategoryDTOService(ICategoryRepository repository, IEmailSender emailSender) : ICategoryDTOService
    {
        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category, string userId)
        {
            Category newCategory = new()
            {
                AppUserId = userId,
                Name = category.Name,
            };
            newCategory = await repository.CreateCategoryAsync(newCategory);
            return newCategory.ToDTO();
        }

        // List of entities and transform them to DTOs
        public async Task<List<CategoryDTO>> GetCategoriesAsync(string userId)
        {
            List<Category> categories = await repository.GetCategoriesAsync(userId);
            return [.. categories.Select(c => c.ToDTO())];
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id, string userId)
        {
            Category? category = await repository.GetCategoryByIdAsync(id, userId);
            return category?.ToDTO() ??
                throw new KeyNotFoundException($"Category with ID {id} not found for user {userId}.");
        }

        public async Task UpdateCategoryAsync(CategoryDTO category, string userId)
        {
            Category? categoryToUpdate = await repository.GetCategoryByIdAsync(category.Id, userId);

            if (categoryToUpdate is not null)
            {
                categoryToUpdate.Name = category.Name;
                categoryToUpdate.Contacts.Clear(); // Clear existing contacts to prevent circular reference

                await repository.UpdateCategoryAsync(categoryToUpdate, userId);
            }

        }
        public async Task DeleteCategoryAsync(int id, string userId)
        {
            await repository.DeleteCategoryAsync(id, userId);
        }

        public async Task<bool> EmailCategoryAsync(int id, EmailData emailData, string userId)
        {
            Category? category = await repository.GetCategoryByIdAsync(id, userId);
            if (category is null || category.Contacts.Count < 1) return false;

            try
            {   //create one string of emails(each separated by a ;) pulled out of the contacts in a category 
                string recipients = string.Join(";", category.Contacts.Select(c => c.Email));
                    await emailSender.SendEmailAsync(recipients, emailData.Subject, emailData.Body);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }

}
