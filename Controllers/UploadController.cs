// File: Controllers/UploadController.cs

using Bina.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bina.Controllers
{
    public class UploadController : Controller
    {
        private readonly FirebaseCloud _firebaseCloud;

        public UploadController(FirebaseCloud firebaseCloud)
        {
            _firebaseCloud = firebaseCloud;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var imageUrl = await _firebaseCloud.UploadFileToFirebase(file);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    ViewBag.Message = "Image uploaded successfully!";
                    ViewBag.ImageUrl = imageUrl;
                }
                else
                {
                    ViewBag.Message = "Image upload failed!";
                }
            }
            else
            {
                ViewBag.Message = "No file selected!";
            }
            return View("Index");
        }
    }
}
