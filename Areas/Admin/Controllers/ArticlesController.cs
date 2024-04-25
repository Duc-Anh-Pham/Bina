using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Bina.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticlesController : Controller
    {
        private readonly Ft1Context _context;

        public ArticlesController(Ft1Context context)
        {
            _context = context;
        }

        // GET: Admin/Articles
        [HttpGet]
        public async Task<IActionResult> Index(string? facultyId = null, int page = 1)
        {
            IQueryable<Article> fT1Context = _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.User);

            if (!string.IsNullOrWhiteSpace(facultyId) && !facultyId.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                fT1Context = fT1Context.Where(a => a.FacultyId == facultyId);
            }

            int pageSize = 4;
            int totalArticles = await fT1Context.CountAsync();
            var articles = await fT1Context.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var pager = new Pager(totalArticles, page, pageSize);

            var model = new Tuple<IEnumerable<Article>, Pager>(articles, pager);

            return View(model);
        }



        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Admin/Articles/Create
        public IActionResult Create()
        {
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId");
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "ArticlesDeadlineId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewBag.FacultyId = new SelectList(_context.Faculties, "FacultyId", "FacultyId");

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,ArticleName,Title,FacultyId")] Article article, DateTime startDue, DateTime dueDate, int academicYear)
        {
            if (ModelState.IsValid)
            {
                if (dueDate <= startDue)
                {
                    ModelState.AddModelError(string.Empty, "Due Date must be later than Start Due.");
                }
                else
                {
                    var articlesDeadline = new ArticlesDeadline
                    {
                        ArticlesDeadlineId = Guid.NewGuid(),
                        StartDue = startDue,
                        DueDate = dueDate,
                        AcademicYear = academicYear,
                    };

                    _context.ArticlesDeadlines.Add(articlesDeadline);
                    await _context.SaveChangesAsync();

                    article.ArticlesDeadlineId = articlesDeadline.ArticlesDeadlineId;

                    _context.Add(article);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            // Truyền lại các giá trị đã nhập vào view
            ViewData["startDue"] = startDue;
            ViewData["dueDate"] = dueDate;
            ViewData["academicYear"] = academicYear;

            // Lấy danh sách faculty từ database và truyền vào view
            ViewBag.FacultyId = new SelectList(await _context.Faculties.ToListAsync(), "FacultyId", "FacultyId", article.FacultyId);

            return View(article);
        }



        // GET: Admin/Articles/Edit

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.ArticlesDeadline)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId");

            return View(article);
        }


        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,ArticleName,Title,Content,FacultyId,ArticlesDeadline")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (article.ArticlesDeadline != null && article.ArticlesDeadline.DueDate > article.ArticlesDeadline.StartDue)
                {
                    try
                    {
                        var existingArticle = await _context.Articles
                            .Include(a => a.ArticlesDeadline)
                            .FirstOrDefaultAsync(a => a.ArticleId == article.ArticleId);

                        if (existingArticle == null)
                        {
                            return NotFound();
                        }

                        // Update in4 article
                        existingArticle.ArticleName = article.ArticleName;
                        existingArticle.Title = article.Title;
                        existingArticle.FacultyId = article.FacultyId;

                        // Update ArticlesDeadline
                        if (article.ArticlesDeadline != null)
                        {
                            if (existingArticle.ArticlesDeadline == null)
                            {
                                existingArticle.ArticlesDeadline = new ArticlesDeadline();
                            }

                            existingArticle.ArticlesDeadline.StartDue = article.ArticlesDeadline.StartDue;
                            existingArticle.ArticlesDeadline.DueDate = article.ArticlesDeadline.DueDate;
                            existingArticle.ArticlesDeadline.AcademicYear = article.ArticlesDeadline.AcademicYear;
                        }

                        _context.Update(existingArticle);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
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
                }
                else //  
                {

                    ModelState.AddModelError(string.Empty, "Due Date must be later than Start Due.");
                }
            }


            if (article.ArticlesDeadline != null)
            {
                ViewData["startDue"] = article.ArticlesDeadline.StartDue;
                ViewData["dueDate"] = article.ArticlesDeadline.DueDate;
                ViewData["academicYear"] = article.ArticlesDeadline.AcademicYear;
                ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", article.FacultyId);
            }


            return View(article);
        }
        // GET: Admin/Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ArticleId == id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }


        // POST: Admin/Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles
                .Include(a => a.ArticlesDeadline)
                .FirstOrDefaultAsync(m => m.ArticleId == id);

            if (article == null)
            {
                return NotFound();
            }

            try
            {
                _context.Articles.Remove(article);

                if (article.ArticlesDeadline != null)
                {
                    var deadlineId = article.ArticlesDeadline.ArticlesDeadlineId;
                    var deadline = await _context.ArticlesDeadlines.FindAsync(deadlineId);
                    if (deadline != null)
                    {
                        _context.ArticlesDeadlines.Remove(deadline);
                    }
                }
                article.FacultyId = null;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }
    }
}
