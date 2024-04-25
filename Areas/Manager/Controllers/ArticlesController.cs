using Bina.Data;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Index(string faculty, string academicYear, string status, int? pageSize = 6, int? pageNumber = 1)
        {
            //ViewBag.Terms = await _context.ArticlesDeadlines.ToListAsync();

            ViewBag.Faculties = await _context.Faculties.ToListAsync();
            ViewBag.Statuses = await _context.ArticleStatuses.ToListAsync();

            var articlesQuery = _context.Articles
                .Include(a => a.ArticleStatus)
                //.Include(a => a.ArticlesDeadline)
                .Include(a => a.Faculty)
                .Include(a => a.User)
                .AsQueryable();

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

            int defaultPageSize = pageSize ?? 5;
            int currentPageNumber = pageNumber ?? 1;
            int totalRecords = await articlesQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / defaultPageSize);

            var filteredArticles = await articlesQuery
                .Skip((currentPageNumber - 1) * defaultPageSize)
                .Take(defaultPageSize)
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPageNumber;
            ViewBag.PageSize = defaultPageSize;

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

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }
    }
}
