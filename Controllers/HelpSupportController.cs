using Bina.Data;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bina.Controllers
{
    /*   public class HelpSupportController : Controller
       {
           public IActionResult HelpSupport()
           {
               return View(new HelpSupport());
           }

           [HttpPost]
           public IActionResult HelpSupport(HelpSupport model)
           {
               if (ModelState.IsValid)
               {
                   return RedirectToAction("ConfirmationPage");
               }
               else
               {
                   return View(model);
               }

           }

           public IActionResult ConfirmationPage()
           {
               return View();
           }
       }*/

    /*
    public class HelpSupportController : Controller
    {
        private readonly ILogger<HelpSupportController> _logger;

        public HelpSupportController(ILogger<HelpSupportController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();

        }
    }
    */
    public class HelpSupportController : Controller
    {
        private readonly ILogger<HelpSupportController> _logger;
        private readonly Ft1Context _ft1;

        public HelpSupportController(ILogger<HelpSupportController> logger, Ft1Context ft1)
        {
            _logger = logger;
            _ft1 = ft1;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult HelpSupport(HelpAndSupport model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                _ft1.HelpAndSupports.Add(model);
                _ft1.SaveChanges();
                return RedirectToAction(nameof(ConfirmationPage));
            }
            return View(model);
        }


        public IActionResult ConfirmationPage()
        {
            return View();
        }
    }
}
