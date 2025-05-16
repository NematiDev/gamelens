using Microsoft.AspNetCore.Mvc;

namespace GameLens.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Manage()
        {
            return View();
        }
    }
}
