using Bina.Data;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bina.Areas.Coordinator.Controllers
{
    [Area("Coordinator")]
    public class ArticlesController : Controller
    {
        private readonly Ft1Context _context;

        public ArticlesController(Ft1Context context)
        {
            _context = context;
        }

        // GET: Coordinator/Articles
        public async Task<IActionResult> Index(string faculty, string term, string academicYear, string status)
        {
            var facultyId = HttpContext.Session.GetString("FacultyId");
            if (string.IsNullOrEmpty(facultyId))
            {
                return RedirectToAction("Login", "Logins");
            }
            ViewBag.FacultyId = facultyId;

            ViewBag.Faculties = await _context.Faculties.ToListAsync();
            ViewBag.Terms = await _context.ArticlesDeadlines.ToListAsync();
            ViewBag.Statuses = await _context.ArticleStatuses.ToListAsync();

            var articlesQuery = _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.Faculty)
                .Include(a => a.User)
                .Where(a => a.Faculty.FacultyId == facultyId); // Chỉ lấy bài báo thuộc facultyId của user

            // Áp dụng các bộ lọc dựa trên tham số đầu vào
            if (!string.IsNullOrEmpty(faculty))
            {
                articlesQuery = articlesQuery.Where(a => a.ArticlesDeadline.FacultyId == faculty);
            }

            if (!string.IsNullOrEmpty(term))
            {
                articlesQuery = articlesQuery.Where(a => a.ArticlesDeadline.TermTitle == term);
            }

            if (!string.IsNullOrEmpty(academicYear))
            {
                articlesQuery = articlesQuery.Where(a => a.ArticlesDeadline.AcademicYear.ToString() == academicYear);
            }

            if (!string.IsNullOrEmpty(status))
            {
                articlesQuery = articlesQuery.Where(a => a.ArticleStatus.ArticleStatusName == status);
            }

            var filteredArticles = await articlesQuery.ToListAsync();
            return View(filteredArticles);
        }


        // GET: Coordinator/Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.ArticleStatus)
                .FirstOrDefaultAsync(m => m.ArticleId == id);

            if (article == null)
            {
                return NotFound();
            }

            // Get all status for dropdown
            ViewBag.StatusList = await _context.ArticleStatuses.ToListAsync();

            return View(article);
        }

        // POST: Article/UpdateStatus
        [HttpPost]
        public IActionResult UpdateStatus(int articleId, int newStatusId)
        {
            var article = _context.Articles.FirstOrDefault(a => a.ArticleId == articleId);
            if (article == null)
            {
                return NotFound();
            }

            var newStatus = _context.ArticleStatuses.FirstOrDefault(s => s.ArticleStatusId == newStatusId);
            if (newStatus == null)
            {
                return BadRequest("Invalid status ID");
            }

            article.ArticleStatusId = newStatusId;
            _context.SaveChanges();

            return Json(new { newStatusName = newStatus.ArticleStatusName });
        }


        // GET: Coordinator/Articles/Create
        public IActionResult Create()
        {
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId");
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "ArticlesDeadlineId");
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Coordinator/Articles/Create
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

        // GET: Coordinator/Articles/Edit/5
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

        // POST: Coordinator/Articles/Edit/5
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

        // GET: Coordinator/Articles/Delete/5
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

        // POST: Coordinator/Articles/Delete/5
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
