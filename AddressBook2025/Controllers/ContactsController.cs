using Microsoft.AspNetCore.Mvc;

namespace AddressBook2025.Controllers
{
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
