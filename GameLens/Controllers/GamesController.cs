using Microsoft.AspNetCore.Mvc;

namespace GameLens.Controllers
{
    public class GamesController : Controller
    {
        public IActionResult Search(string query, int page)
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
