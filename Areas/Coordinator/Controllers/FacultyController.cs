using Bina.Data;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bina.Models.Authentication;

namespace Bina.Areas.Coordinator.Controllers
{
    [Area("Coordinator")]
    public class FacultyController : Controller
    {
        private readonly Ft1Context _context;

        public FacultyController(Ft1Context context)
        {
            _context = context;
        }

        [Authentication]

        // GET: Faculty
        public async Task<IActionResult> Index()
        {
            if (_context.Faculties == null)
            {
                return Problem("Entity set 'FT1Context.Faculties' is null.");
            }

            var facultiesWithCoordinators = await _context.Faculties
                .Select(faculty => new ViewModels
                {
                    FacultyId = faculty.FacultyId,
                    FacultyName = faculty.FacultyName,
                    Established = faculty.Established ?? default(DateTime),
                    CoordinatorUserName = _context.Users
                        .Where(u => u.FacultyId == faculty.FacultyId && u.Role.RoleName == "Marketing Coordinator")
                        .Select(u => u.UserName)
                        .FirstOrDefault() // Assuming there's only one coordinator per faculty
                }).ToListAsync();

            return View(facultiesWithCoordinators);
        }

        // GET: Faculty/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Faculties == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
               .Select(f => new ViewModels
               {
                   FacultyId = f.FacultyId,
                   FacultyName = f.FacultyName,
                   Established = f.Established,
                   CoordinatorUserName = _context.Users
                       .Where(u => u.FacultyId == f.FacultyId && u.Role.RoleName == "Marketing coordinator")
                       .Select(u => u.UserName)
                       .FirstOrDefault()
               })
               .FirstOrDefaultAsync(f => f.FacultyId == id);
            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // GET: Faculty/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Faculty/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacultyId,FacultyName,Established")] Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faculty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faculty);
        }
        // GET: Faculty/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Faculties == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
                .Select(f => new ViewModels
                {
                    FacultyId = f.FacultyId,
                    FacultyName = f.FacultyName,
                    Established = f.Established,
                    CoordinatorUserName = _context.Users
                        .Where(u => u.FacultyId == f.FacultyId && u.Role.RoleName == "Marketing coordinator")
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(f => f.FacultyId == id);

            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // POST: Faculty/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("FacultyId,FacultyName,Established,CoordinatorUserName")] ViewModels viewModel)
        {
            if (id != viewModel.FacultyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // First, update the faculty details
                    var faculty = await _context.Faculties.FindAsync(viewModel.FacultyId);
                    if (faculty == null)
                    {
                        return NotFound();
                    }

                    faculty.FacultyName = viewModel.FacultyName;
                    faculty.Established = viewModel.Established;
                    _context.Update(faculty);

                    // Then, update the coordinator user name if it has changed
                    var currentCoordinator = await _context.Users
                        .Where(u => u.FacultyId == viewModel.FacultyId && u.Role.RoleName == "Marketing coordinator")
                        .FirstOrDefaultAsync();

                    if (currentCoordinator != null && currentCoordinator.UserName != viewModel.CoordinatorUserName)
                    {
                        // Here you would ideally check if the new username exists or any business logic around changing usernames
                        currentCoordinator.UserName = viewModel.CoordinatorUserName;
                        _context.Update(currentCoordinator);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyExists(viewModel.FacultyId))
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
            return View(viewModel);
        }


        // GET: Faculty/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Faculties == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
                .Select(f => new ViewModels
                {
                    FacultyId = f.FacultyId,
                    FacultyName = f.FacultyName,
                    Established = f.Established,
                    CoordinatorUserName = _context.Users
                        .Where(u => u.FacultyId == f.FacultyId && u.Role.RoleName == "Marketing coordinator")
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(f => f.FacultyId == id);

            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // POST: Faculty/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Faculties == null)
            {
                return Problem("Entity set 'FT1Context.Faculties' is null.");
            }
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty != null)
            {
                _context.Faculties.Remove(faculty);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FacultyExists(string id)
        {
            return (_context.Faculties?.Any(e => e.FacultyId == id)).GetValueOrDefault();
        }
    }
}