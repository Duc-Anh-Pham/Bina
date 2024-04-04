/*Bina / Controllers / ArticlesController.cs*/
using Bina.Data;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bina.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly Ft1Context _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }

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
                    // Lưu file dưới dạng BLOB trong cột 'Content'
                    using (var memoryStream = new MemoryStream())
                    {
                        await uploadFile.CopyToAsync(memoryStream);
                        article.Content = memoryStream.ToArray();
                    }
                }

                if (uploadImage != null && uploadImage.Length > 0)
                {
                    // Lưu file ảnh vào thư mục trên server
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadImage.CopyToAsync(fileStream);
                    }

                    // Lưu đường dẫn file ảnh vào cơ sở dữ liệu, nếu cần
                    var image = new Image { Imagepath = "/uploads/" + fileName };
                    _context.Images.Add(image);
                    await _context.SaveChangesAsync(); // This saves the image record and generates the ImageID

                    article.ImageId = image.ImageId; // Assign the generated ImageId to the article
                }

                // Thêm bài viết vào cơ sở dữ liệu
                _context.Articles.Add(article);
                await _context.SaveChangesAsync(); // This saves the article record, which includes the Content BLOB and ImageID
                return RedirectToAction(nameof(Index));
            }

            // Prepare ViewData for the view if the model state is not valid
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusName", article.ArticleStatusId);
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "DueDate", article.ArticlesDeadlineId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", article.FacultyId);
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
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,ArticleName,Title,Content,UserId,ImageId,ArticleStatusId,ArticlesDeadlineId,FacultyId")] Article article, IFormFile uploadFile, IFormFile uploadImage)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadFile != null && uploadFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await uploadFile.CopyToAsync(memoryStream);
                            article.Content = memoryStream.ToArray();
                        }
                    }

                    if (uploadImage != null && uploadImage.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadImage.CopyToAsync(fileStream);
                        }

                        if (article.Image != null)
                        {
                            _context.Images.Remove(article.Image); // Xoá hình ảnh cũ
                        }

                        var image = new Image { Imagepath = "/uploads/" + fileName };
                        _context.Images.Add(image);
                        await _context.SaveChangesAsync();

                        article.ImageId = image.ImageId;
                    }

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
            // Cập nhật SelectList cho View
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusName", article.ArticleStatusId);
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "DueDate", article.ArticlesDeadlineId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", article.FacultyId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "Imagepath", article.ImageId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName", article.UserId);
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
            var article = await _context.Articles.Include(a => a.Image).FirstOrDefaultAsync(a => a.ArticleId == id);
            if (article != null)
            {
                // Xoá file ảnh từ server
                if (article.Image != null)
                {
                    var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, article.Image.Imagepath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    _context.Images.Remove(article.Image);
                }

                // Xoá file nếu bài viết có file đính kèm
                if (article.Content != null)
                {
                    // Tùy thuộc vào cách bạn lưu trữ file, nếu bạn lưu trữ file trên server, xoá file tại đây
                    // Ví dụ: System.IO.File.Delete(ServerFilePath);
                }

                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }




    }
}
