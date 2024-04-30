using Microsoft.AspNetCore.Mvc;

namespace Bina.Areas.Coordinator.Controllers
{
    public class News : Controller
    {
        public IActionResult ListNews()
        {
            return View();
        }

        public IActionResult ContentNews()
        {
            return View();
        }
    }
}
