using AddressBook2025.Client.Services.Interfaces;
using AddressBook2025.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AddressBook2025.Data;
using AddressBook2025.Client.Models.DTOs;

namespace AddressBook2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController(ICategoryDTOService categoryService, UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private string _userId => userManager.GetUserId(User)!; //[Authorize] ensures User is not null, so ! is safe

        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategoriesAsync()
        {
            try
            {
                return await categoryService.GetCategoriesAsync(_userId);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return Problem();
            }
        }
        [HttpGet("id")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryAsync(int id)
        {
            try
            {
                CategoryDTO? category = await categoryService.GetCategoryByIdAsync(id, _userId);
                return category is null ?
                    NotFound($"Category with ID {id} not found for user {_userId}.")
                    : Ok(category);

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }


        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategoryAsync(CategoryDTO category)
        {
            try
            {
                CategoryDTO createdCategory = await categoryService.CreateCategoryAsync(category, _userId);
                return CreatedAtAction(nameof(GetCategoryAsync), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateCategoryAsync([FromRoute] int id, [FromBody] CategoryDTO category)
        {
            if (id != category.Id) return BadRequest();
            try
            {
                await categoryService.UpdateCategoryAsync(category, _userId);
                return Ok();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return Problem();
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCategoryAsync([FromRoute] int id)
        {
            try
            {
                await categoryService.DeleteCategoryAsync(id, _userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }
    }
}
