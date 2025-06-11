using AddressBook2025.Client.Services.Interfaces;
using AddressBook2025.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AddressBook2025.Client.Models.DTOs;

namespace AddressBook2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController(IContactDTOService contactService, UserManager<Data.ApplicationUser> userManager) : ControllerBase
    {
        //Since[Authorize], userId cannot be null ; always checked whenever userId is called
        private string UserId => userManager.GetUserId(User)!;

        #region ENDPOINT METHODS MIRRORING SERVICE METHODS
        [HttpGet]
        public async Task<ActionResult<List<ContactDTO>>> GetContacts([FromQuery] int? categoryId)
        {
            try
            {
                if (categoryId is not null or 0)
                {
                    return await contactService.GetContactsByCategoryAsync(categoryId.Value, UserId);
                }
                else
                {
                    return await contactService.GetContactsAsync(UserId);
                }
            }
            catch (Exception ex)
            {

               Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ContactDTO>>> SearchContacts([FromQuery] string query)
        {
            if(string.IsNullOrEmpty(query)) return BadRequest();
            try
            {
                return await contactService.SearchContactsAsync(query, UserId);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                    return Problem();
            }  
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteContact([FromRoute] int id)
        {
            try
            {
                await contactService.DeleteContactAsync(id, UserId);
                return NoContent();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return Problem();
            }
        }
        #endregion
    }
}
