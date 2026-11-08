using Microsoft.AspNetCore.Mvc;

namespace PharmaERP.Identity.API.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
