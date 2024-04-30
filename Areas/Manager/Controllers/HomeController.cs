using Bina.Models;
using Bina.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bina.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authentication]

        public IActionResult Index()
        {
            return View();
        }

        [Authentication]

        public IActionResult Privacy()
        {
            return View();
        }

        [Authentication]

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
