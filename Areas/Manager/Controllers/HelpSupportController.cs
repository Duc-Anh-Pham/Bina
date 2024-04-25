using Bina.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bina.Areas.Manager.Controllers
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
    [Area("Manager")]
    public class HelpSupportController : Controller
    {
        private readonly ILogger<HelpSupportController> _logger; //ILogger ghi log cho HelpSupportController

        public HelpSupportController(ILogger<HelpSupportController> logger)
        {
            _logger = logger; // gán giá trị _logger cho Ilogger
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); //hiển thị form gửi support
        }

        [HttpPost]
        public IActionResult HelpSupport(HelpSupport model)
        {
            if (ModelState.IsValid) //nếu các phương thức nhập dữ liệu hợp lệ thì trả về trang ConfirmationPage
            {

                return RedirectToAction("ConfirmationPage");
            }
            else
            {
                return View("Index", model);
            }
        }

        public IActionResult ConfirmationPage()
        {
            return View();
        }
    }
}
