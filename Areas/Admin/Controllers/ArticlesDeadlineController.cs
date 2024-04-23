using Bina.Data;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bina.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticlesDeadlineController : Controller
    {
        private readonly Ft1Context _context;

        public ArticlesDeadlineController(Ft1Context context)
        {
            _context = context;
        }

        // GET: Admin/ArticlesDeadline
        public async Task<IActionResult> Index(string? facultyId = null, int page = 1)
        {
            IQueryable<ArticlesDeadline> query = _context.ArticlesDeadlines
                .Include(ad => ad.Faculty)
                .Include(ad => ad.User);

            if (!string.IsNullOrWhiteSpace(facultyId) && !facultyId.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(ad => ad.FacultyId == facultyId);
            }

            int pageSize = 4;
            int totalEntries = await query.CountAsync();
            List<ArticlesDeadline> deadlines = await query
                .OrderBy(ad => ad.StartDue)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pager = new Pager(totalEntries, page, pageSize);
            var model = new Tuple<IEnumerable<ArticlesDeadline>, Pager>(deadlines, pager);

            return View(model);
        }

        // GET: Admin/ArticlesDeadline/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var deadline = await _context.ArticlesDeadlines
                .Include(ad => ad.Faculty)
                .Include(ad => ad.User)
                .FirstOrDefaultAsync(m => m.ArticlesDeadlineId == id);
            if (deadline == null)
            {
                return NotFound();
            }
            return View(deadline);
        }

        // GET: Admin/ArticlesDeadline/Create
        public IActionResult Create()
        {
            ViewBag.FacultyId = new SelectList(_context.Faculties, "FacultyId", "FacultyId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name");
            return View();
        }

        // POST: Admin/ArticlesDeadline/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TermName,TermTitle,StartDue,DueDate,AcademicYear,FacultyId")] ArticlesDeadline deadline)
        {
            if (ModelState.IsValid)
            {
                if (deadline.DueDate <= deadline.StartDue)
                {
                    ModelState.AddModelError(string.Empty, "Due Date must be later than Start Due.");
                }
                else
                {
                    _context.Add(deadline);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // Truyền lại các giá trị đã nhập vào view
            ViewData["startDue"] = deadline.StartDue;
            ViewData["dueDate"] = deadline.DueDate;
            ViewData["academicYear"] = deadline.AcademicYear;

            // Lấy danh sách faculty từ database và truyền vào view
            ViewBag.FacultyId = new SelectList(await _context.Faculties.ToListAsync(), "FacultyId", "FacultyId", deadline.FacultyId);

            return View(deadline);
        }


        // GET: Admin/ArticlesDeadline/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var deadline = await _context.ArticlesDeadlines.FindAsync(id);
            if (deadline == null)
            {
                return NotFound();
            }
            ViewBag.FacultyId = new SelectList(_context.Faculties, "FacultyId", "FacultyId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", deadline.UserId);
            return View(deadline);
        }

        // POST: Admin/ArticlesDeadline/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ArticlesDeadlineId,StartDue,DueDate,AcademicYear,TermName,TermTitle,FacultyId,UserId")] ArticlesDeadline deadline)
        {
            if (id != deadline.ArticlesDeadlineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (deadline.StartDue > deadline.DueDate)
                {
                    ModelState.AddModelError(string.Empty, "Start Due must be earlier than Due Date.");
                }
                else
                {
                    try
                    {
                        _context.Update(deadline);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!DeadlineExists(deadline.ArticlesDeadlineId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            ViewBag.FacultyId = new SelectList(_context.Faculties, "FacultyId", "FacultyId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Name", deadline.UserId);
            return View(deadline);
        }


        // GET: Admin/ArticlesDeadline/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deadline = await _context.ArticlesDeadlines
                .Include(ad => ad.Faculty)
                .Include(ad => ad.User)
                .FirstOrDefaultAsync(m => m.ArticlesDeadlineId == id);

            if (deadline == null)
            {
                return NotFound();
            }

            return View(deadline);
        }

        // POST: Admin/ArticlesDeadline/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var deadline = await _context.ArticlesDeadlines
                .Include(ad => ad.Articles)
                .FirstOrDefaultAsync(m => m.ArticlesDeadlineId == id);

            if (deadline == null)
            {
                return NotFound();
            }

            try
            {
                // Xóa tất cả các bài báo có liên kết với deadline trước
                foreach (var article in deadline.Articles)
                {
                    article.ArticlesDeadlineId = null;
                    _context.Articles.Update(article);
                }

                // Tiến hành xóa deadline
                _context.ArticlesDeadlines.Remove(deadline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }


        private bool DeadlineExists(Guid id)
        {
            return _context.ArticlesDeadlines.Any(e => e.ArticlesDeadlineId == id);
        }
    }
}
