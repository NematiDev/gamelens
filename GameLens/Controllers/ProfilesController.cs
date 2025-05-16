using Microsoft.AspNetCore.Mvc;

namespace GameLens.Controllers
{
    public class ProfilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
