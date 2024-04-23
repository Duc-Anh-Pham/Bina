using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bina.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var cookies = HttpContext.Request.Cookies;
            var cookieList = new List<string>();

            foreach (var cookie in cookies)
            {
                cookieList.Add($"{cookie.Key}: {cookie.Value}");
            }

            return View(cookieList);
        }

        public IActionResult Privacy()
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
