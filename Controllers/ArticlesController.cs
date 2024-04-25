using Bina.Data;
using Bina.Models;
using Bina.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bina.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly Ft1Context _context;
        private readonly FirebaseCloud _firebaseCloud;

        public ArticlesController(Ft1Context context, ILogger<ArticlesController> logger, FirebaseCloud firebaseCloud)
        {
            _context = context;
            _firebaseCloud = firebaseCloud;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Xử lý trường hợp không tìm thấy UserId, có thể là chưa đăng nhập
                return RedirectToAction("Login", "Logins");
            }

            var articles = _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.Faculty)
                .Include(a => a.User)
                .Where(a => a.UserId == userId);  // Lọc các bài viết theo UserId

            return View(await articles.ToListAsync());
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
            // Get UserId from session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // If UserId is not found, possibly user is not logged in
                return RedirectToAction("Login", "Logins");
            }

            var user = _context.Users
            .Include(u => u.Faculty)
            .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.FacultyId == null)
            {
                // If user or user's faculty is not found
                return RedirectToAction("ErrorPage", new { message = "Your faculty information is missing." });
            }

            // Get the current deadline for the user's faculty
            var currentDeadline = _context.ArticlesDeadlines
                .FirstOrDefault(ad => ad.FacultyId == user.FacultyId && ad.DueDate >= DateTime.Now);

            if (currentDeadline == null)
            {
                // If current date is past the DueDate or there's no active deadline
                return RedirectToAction("ErrorPage", new { message = "Hiện tại Khoa này đã đóng chức năng Nộp bài Article, vui lòng liên hệ Coordinator nếu cần hỗ trợ thêm" });
            }

            // Lấy danh sách deadlines liên quan đến khoa của người dùng
            var deadlines = _context.ArticlesDeadlines
                .Where(a => a.FacultyId == user.FacultyId)
                .OrderBy(a => a.DueDate) // Sắp xếp theo ngày hạn chót để lấy ra hạn chót gần nhất
                .ToList(); // Lấy ra tất cả mà không chỉ là đầu tiên

            // Lấy danh sách deadlines theo FacultyId của người dùng
            var faculties = _context.Faculties
           .Where(f => f.FacultyId == user.FacultyId) // Chỉ lấy khoa mà người dùng thuộc về
           .ToList();

            // Get the current deadlines for the user's faculty that are still valid
            var validDeadlines = _context.ArticlesDeadlines
                .Where(ad => ad.FacultyId == user.FacultyId && ad.DueDate >= DateTime.Now)
                .OrderBy(ad => ad.DueDate)
                .ToList();

            if (!validDeadlines.Any())
            {
                // If there are no valid deadlines
                return RedirectToAction("ErrorPage", new { message = "Hiện tại Khoa này đã đóng chức năng Nộp bài Article, vui lòng liên hệ Coordinator nếu cần hỗ trợ thêm" });
            }

            // Create a SelectList for the current valid deadlines
            var deadlineTermsSelectList = validDeadlines.Select(ad => new SelectListItem
            {
                Value = ad.ArticlesDeadlineId.ToString(),
                Text = $"{ad.TermTitle} - From {ad.StartDue?.ToString("dd/MM/yyyy")} => To {ad.DueDate?.ToString("HH:mm MMMM dd, yyyy")}"
            }).ToList();


            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId");
            ViewData["ArticlesDeadlineId"] = new SelectList(deadlines, "ArticlesDeadlineId", "TermTitle");
            ViewData["FacultyId"] = new SelectList(faculties, "FacultyId", "FacultyId", user.FacultyId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", userId);
            ViewData["DeadlineTerms"] = deadlineTermsSelectList;

            var article = new Article
            {
                UserId = userId,
                ArticleStatusId = 3,
                FacultyId = user.FacultyId, // Đặt khoa mặc định cho bài viết dựa trên khoa của người dùng
                ArticlesDeadlineId = validDeadlines.First().ArticlesDeadlineId
            };

            // Lấy deadline đầu tiên phù hợp hoặc null nếu không có
            /* article.ArticlesDeadlineId = deadlines.FirstOrDefault()?.ArticlesDeadlineId;*/


            return View(article);
        }

        // POST: Articles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article, IFormFile imageFile, IFormFile documentFile)
        {
            if (ModelState.IsValid)
            {
                // Xử lý file ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageUrl = await _firebaseCloud.UploadFileToFirebase(imageFile);
                    article.ImagePath = imageUrl; // Lưu URL của ảnh vào thuộc tính ImagePath
                }

                // Xử lý file tài liệu
                if (documentFile != null && documentFile.Length > 0)
                {
                    var documentUrl = await _firebaseCloud.UploadFileToFirebase(documentFile);
                    article.DocumentPath = documentUrl; // Lưu URL của tài liệu vào thuộc tính DocumentPath
                }

                _context.Add(article);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", article.UserId);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,ArticleName,Title,Content,UserId,ImagePath,DocumentPath,ArticleStatusId,ArticlesDeadlineId,FacultyId")] Article article)
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

        // Custom error page action
        public IActionResult ErrorPage(string message)
        {
            ViewData["ErrorMessage"] = message;
            return View();
        }
    }
}
