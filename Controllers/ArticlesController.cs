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
            // Lấy UserId từ session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Nếu không tìm thấy UserId, có thể chuyển hướng người dùng đến trang đăng nhập
                return RedirectToAction("Login", "Logins");
            }

            var user = _context.Users
            .Include(u => u.Faculty) // Giả sử mỗi người dùng liên kết với một khoa
            .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.FacultyId == null)
            {
                // Xử lý nếu không tìm thấy người dùng hoặc người dùng không có khoa nào
                return RedirectToAction("Login", "Logins"); // Hoặc trả về một thông báo lỗi phù hợp
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

            // Tạo SelectList cho các deadlines bao gồm ngày bắt đầu và kết thúc
            var deadlineTermsSelectList = deadlines.Select(ad => new SelectListItem
            {
                Value = ad.ArticlesDeadlineId.ToString(),
                Text = $"{ad.TermTitle} - từ {ad.StartDue?.ToString("dd/MM/yyyy")} đến {ad.DueDate?.ToString("dd/MM/yyyy")}"
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
                FacultyId = user.FacultyId // Đặt khoa mặc định cho bài viết dựa trên khoa của người dùng
            };

            // Lấy deadline đầu tiên phù hợp hoặc null nếu không có
            article.ArticlesDeadlineId = deadlines.FirstOrDefault()?.ArticlesDeadlineId;


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
    }
}
