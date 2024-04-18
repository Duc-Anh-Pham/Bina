using Bina.Data;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bina.Controllers
{
    public class ArticlesFacultyController : Controller
    {
        private readonly Ft1Context _context;

        public ArticlesFacultyController(Ft1Context context)
        {
            _context = context;
        }

        // GET: ArticlesFaculty
        public async Task<IActionResult> Index()
        {
            var facultyId = HttpContext.Session.GetString("FacultyId");
            if (string.IsNullOrEmpty(facultyId))
            {
                // Handle case when no facultyId is set, maybe redirect to an error page or a default view.
                return RedirectToAction("Login", "Logins"); // Or another appropriate response
            }

            var ft1Context = _context.Articles
                                     .Include(a => a.ArticleStatus)
                                     .Include(a => a.ArticlesDeadline)
                                     .Include(a => a.Faculty)
                                     .Include(a => a.User)
                                     .Where(a => a.Faculty.FacultyId == facultyId);

            return View(await ft1Context.ToListAsync());
        }


        // GET: ArticlesFaculty/Details/5
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

        // GET: ArticlesFaculty/Create
        public IActionResult Create()
        {
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId");
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "ArticlesDeadlineId");
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: ArticlesFaculty/Create
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

        // GET: ArticlesFaculty/Edit/5
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

        // POST: ArticlesFaculty/Edit/5
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

        // GET: ArticlesFaculty/Delete/5
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

        // POST: ArticlesFaculty/Delete/5
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
