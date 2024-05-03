using Bina.Data;
using Bina.Models;
using Bina.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace Bina.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class ArticlesController : Controller
    {
        private readonly Ft1Context _context;

        public ArticlesController(Ft1Context context)
        {
            _context = context;
        }

        // GET: Manager/Articles
        public async Task<IActionResult> Index(string faculty, string academicYear, string status, int page = 1, int pageSize = 9)
        {
            // Fetching filter-related lists
            ViewBag.Faculties = await _context.Faculties.ToListAsync();

            ViewBag.Statuses = await _context.ArticleStatuses.ToListAsync();
            ViewBag.FacultyFilter = faculty;
            ViewBag.AcademicYearFilter = academicYear;
            ViewBag.StatusFilter = status;
            var articlesQuery = _context.Articles
                 .Include(a => a.ArticleStatus)
                 .Include(a => a.ArticlesDeadline)
                 .Include(a => a.Faculty)
                 .Include(a => a.User)
                  .AsQueryable();

            // Applying filters
            if (!string.IsNullOrEmpty(faculty))
            {
                articlesQuery = articlesQuery.Where(a => a.ArticlesDeadline.FacultyId == faculty);
            }



            if (!string.IsNullOrEmpty(academicYear))
            {
                articlesQuery = articlesQuery.Where(a => a.ArticlesDeadline.AcademicYear.ToString() == academicYear);
            }

            if (!string.IsNullOrEmpty(status))
            {
                articlesQuery = articlesQuery.Where(a => a.ArticleStatus.ArticleStatusName == status);
            }


            int totalRecords = await articlesQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));  // Ensure the page is within the valid range

            var filteredArticles = await articlesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(filteredArticles);
        }

        // GET: Manager/Articles/Details/5
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
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Manager/Articles/Create
        public IActionResult Create()
        {
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId");
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "ArticlesDeadlineId");
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Manager/Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,ArticleName,Title,Content,UserId,ImagePath,DocumentPath,DateCreate,ArticleStatusId,ArticlesDeadlineId,FacultyId")] Article article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId", article.ArticleStatusId);
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "ArticlesDeadlineId", article.ArticlesDeadlineId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", article.FacultyId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", article.UserId);
            return View(article);
        }

        // GET: Manager/Articles/Edit/5
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", article.UserId);
            return View(article);
        }

        // POST: Manager/Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,ArticleName,Title,Content,UserId,ImagePath,DocumentPath,DateCreate,ArticleStatusId,ArticlesDeadlineId,FacultyId")] Article article)
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", article.UserId);
            return View(article);
        }

        // GET: Manager/Articles/Delete/5
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
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Manager/Articles/Delete/5
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

        [HttpPost]
        public async Task<IActionResult> ExportToZip(string faculty, string academicYear, string status)
        {
            var articlesQuery = _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.Faculty)
                .Include(a => a.User)
                .AsQueryable();

            // Áp dụng bộ lọc (nếu có)
            if (!string.IsNullOrEmpty(faculty))
            {
                articlesQuery = articlesQuery.Where(a => a.ArticlesDeadline.FacultyId == faculty);
            }

            if (!string.IsNullOrEmpty(academicYear))
            {
                articlesQuery = articlesQuery.Where(a => a.ArticlesDeadline.AcademicYear.ToString() == academicYear);
            }

            if (!string.IsNullOrEmpty(status))
            {
                articlesQuery = articlesQuery.Where(a => a.ArticleStatus.ArticleStatusName == status);
            }

            var articles = await articlesQuery.ToListAsync();

            // Tạo tệp ZIP
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var article in articles)
                    {
                        if (!string.IsNullOrEmpty(article.DocumentPath))
                        {
                            byte[] fileBytes;

                            // Kiểm tra đường dẫn có phải là URL hay không
                            if (Uri.TryCreate(article.DocumentPath, UriKind.Absolute, out Uri uri))
                            {
                                // Nếu là URL, tải nội dung từ URL
                                using (var client = new HttpClient())
                                {
                                    var response = await client.GetAsync(uri);
                                    response.EnsureSuccessStatusCode();
                                    fileBytes = await response.Content.ReadAsByteArrayAsync();
                                }
                            }
                            else
                            {
                                // Nếu không phải là URL, đọc nội dung từ đường dẫn tệp vật lý
                                fileBytes = System.IO.File.ReadAllBytes(article.DocumentPath);
                            }

                            var fileName = Path.GetFileName(article.DocumentPath);
                            var zipEntry = zipArchive.CreateEntry(fileName, CompressionLevel.Optimal);

                            using (var entryStream = zipEntry.Open())
                            {
                                entryStream.Write(fileBytes, 0, fileBytes.Length);
                            }
                        }
                    }
                }

                // Lưu tệp ZIP vào đĩa
                string zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), "articles.zip");
                System.IO.File.WriteAllBytes(zipFilePath, memoryStream.ToArray());

                // Giải nén tệp ZIP
                ExtractZipFile(zipFilePath);

                // Trả về tệp ZIP cho người dùng (tùy chọn)
                return File(memoryStream.ToArray(), "application/zip", "articles.zip");
            }
        }

        public IActionResult ExtractZipFile(string zipFilePath)
        {
            try
            {
                // Đường dẫn thư mục lưu trữ các tệp PDF đã giải nén
                string extractPath = Path.Combine(Directory.GetCurrentDirectory(), "ExtractedPDFs");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                // Giải nén tệp ZIP
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string filePath = Path.Combine(extractPath, entry.FullName);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        entry.ExtractToFile(filePath, true);
                    }
                }

                // Trả về thông báo thành công hoặc thực hiện các hành động khác
                return Content("Tệp ZIP đã được giải nén thành công.");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return Content($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }
    }
}
