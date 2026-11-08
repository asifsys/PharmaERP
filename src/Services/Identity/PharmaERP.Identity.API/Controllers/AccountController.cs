using Microsoft.AspNetCore.Mvc;

namespace PharmaERP.Identity.API.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
