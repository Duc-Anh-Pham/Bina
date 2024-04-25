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
                return RedirectToAction("Login", "Logins");
            }

            var articles = _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.Faculty)
                .Include(a => a.User)
                .Where(a => a.UserId == userId);

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
                .Include(a => a.CommentFeedbacks)
                            .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedback(int? articleId, string feedbackText)
        {
            if (!ModelState.IsValid || articleId == null || string.IsNullOrWhiteSpace(feedbackText))
            {
                articleId = 1;
                feedbackText = "No feedback";
            }

            var userId = GetCurrentUserIdFromSession();

            var newFeedback = new CommentFeedback
            {
                CommentFeedbackId = Guid.NewGuid(),
                ArticleId = articleId.Value,

                UserId = userId.Value,
                ContentFeedback = feedbackText,
                CommentDay = DateTime.Now
            };

            _context.CommentFeedbacks.Add(newFeedback);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }


            return RedirectToAction("Details", "Articles", new { id = articleId.Value });
        }
        private int? GetCurrentUserIdFromSession()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId;
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Logins");
            }

            var user = _context.Users
            .Include(u => u.Faculty)
            .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.FacultyId == null)
            {
                return RedirectToAction("Login", "Logins");
            }

            var deadlines = _context.ArticlesDeadlines
               .Where(a => a.FacultyId == user.FacultyId)
               .OrderBy(a => a.DueDate)
               .ToList();

            var faculties = _context.Faculties
          .Where(f => f.FacultyId == user.FacultyId)
          .ToList();

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
                FacultyId = user.FacultyId
            };

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
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageUrl = await _firebaseCloud.UploadFileToFirebase(imageFile);
                    article.ImagePath = imageUrl;
                }

                // Xử lý file tài liệu
                if (documentFile != null && documentFile.Length > 0)
                {
                    var documentUrl = await _firebaseCloud.UploadFileToFirebase(documentFile);
                    article.DocumentPath = documentUrl;
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
