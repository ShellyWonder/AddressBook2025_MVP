using AddressBook2025.Client.Services.Interfaces;
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContactDTO>> GetContactByIdAsync([FromRoute] int id)
        {
            try
            {
                ContactDTO contact = await contactService.GetContactByIdAsync(id,UserId);
                return contact is null ? NotFound() : contact;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);

                return Problem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContactDTO>> CreateContact([FromBody] ContactDTO contact)
        {
            try
            {
                ContactDTO newContact = await contactService.CreateContactAsync(contact, UserId);
                    return CreatedAtAction(nameof(GetContactByIdAsync), new {id = newContact.Id},newContact);
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
