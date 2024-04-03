/*Bina / Controllers / ArticlesController.cs*/
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bina.Data;
using Bina.Models;

namespace Bina.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly Ft1Context _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ArticlesController(Ft1Context context, IWebHostEnvironment hostingEnvironment) // Modify the constructor
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment; // Set the environment
        }

        public async Task<IActionResult> GetFile(int id)
        {
            var article = await _context.Articles
                .FirstOrDefaultAsync(a => a.ArticleId == id);
            if (article == null || article.Content == null)
            {
                return NotFound();
            }

            // Giả định bạn biết loại file hoặc có cột trong cơ sở dữ liệu để xác định loại file
            var fileExtension = ".pdf";  // Thay đổi dựa vào loại file thực tế hoặc lấy từ cơ sở dữ liệu
            var fileName = $"Article_{article.ArticleId}{fileExtension}";  // Tạo tên file dựa vào ID của bài viết

            // Bạn cần xác định loại MIME type chính xác cho loại file
            // Ví dụ: "application/pdf" cho PDF, "image/jpeg" cho JPEG, v.v.
            var contentType = "application/pdf"; // Thay đổi này dựa vào loại file thực tế

            var stream = new MemoryStream(article.Content);
            return File(stream, contentType, fileName);
        }



        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var ft1Context = _context.Articles.Include(a => a.ArticleStatus).Include(a => a.ArticlesDeadline).Include(a => a.Faculty).Include(a => a.Image).Include(a => a.User);
            return View(await ft1Context.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.Faculty)
                .Include(a => a.Image)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusName");
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "DueDate");
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName");
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "Imagepath");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,ArticleName,Title,UserId,ArticleStatusId,ArticlesDeadlineId,FacultyId")] Article article, IFormFile uploadFile, IFormFile uploadImage)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null && uploadFile.Length > 0)
                {
                    // Lưu file document như BLOB vào cột 'Content'
                    using (var memoryStream = new MemoryStream())
                    {
                        await uploadFile.CopyToAsync(memoryStream);
                        article.Content = memoryStream.ToArray();
                    }
                }

                if (uploadImage != null && uploadImage.Length > 0)
                {
                    // Tạo tên file duy nhất để tránh xung đột
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "avatars", fileName);

                    // Tạo thư mục nếu chưa tồn tại
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // Lưu file ảnh vào thư mục 'uploads/avatars'
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadImage.CopyToAsync(fileStream);
                    }

                    // Tạo đường dẫn đầy đủ cho ảnh avatar
                    var domain = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                    var urlPath = $"{domain}/uploads/avatars/{fileName}";

                    // Tạo mới hoặc cập nhật bản ghi Image với đường dẫn đầy đủ
                    var existingImage = _context.Images.FirstOrDefault(img => img.Imagepath == urlPath);
                    if (existingImage == null)
                    {
                        var image = new Image { Imagepath = urlPath };
                        _context.Images.Add(image);
                        await _context.SaveChangesAsync();
                        article.ImageId = image.ImageId; // Chỉ định ImageId mới tạo cho bài viết
                    }
                    else
                    {
                        article.ImageId = existingImage.ImageId; // Sử dụng ImageId hiện có
                    }
                }

                // Thêm article vào database
                _context.Articles.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, chuẩn bị lại ViewData cho các dropdown
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusName", article.ArticleStatusId);
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "DueDate", article.ArticlesDeadlineId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", article.FacultyId);
            // Bỏ đi vì không cần thiết và có thể gây nhầm lẫn: ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "Imagepath", article.ImageId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName", article.UserId);

            return View(article);
        }


        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId", article.ArticleStatusId);
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "ArticlesDeadlineId", article.ArticlesDeadlineId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", article.FacultyId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", article.ImageId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", article.UserId);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,ArticleName,Title,Content,UserId,ImageId,ArticleStatusId,ArticlesDeadlineId,FacultyId")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId", article.ArticleStatusId);
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "ArticlesDeadlineId", article.ArticlesDeadlineId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", article.FacultyId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", article.ImageId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", article.UserId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.Faculty)
                .Include(a => a.Image)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }
    }
}
