using Bina.Data;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Bina.Controllers
{
    [Area("Admin")]
    public class FacultyController : Controller
    {
        private readonly Ft1Context _context;

        public FacultyController(Ft1Context context)
        {
            _context = context;
        }

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
                    Status = faculty.Status,
                    CoordinatorUserName = _context.Users
                        .Where(u => u.FacultyId == faculty.FacultyId && u.Role.RoleName == "Marketing Coordinator")
                        .Select(u => u.UserName)
                        .FirstOrDefault()
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
            ViewBag.CoordinatorUserNames = _context.Users
           .Where(u => u.RoleId == 2 && u.FacultyId == null)
           .Select(u => new SelectListItem
           {
               Text = u.UserName,
               Value = u.UserName
           })
           .ToList();

            return View();
        }

        // POST: Faculty/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacultyId,FacultyName,Established,CoordinatorUserName")] ViewModels viewModel)
        {
            if (ModelState.IsValid)
            {
                var faculty = new Faculty
                {
                    FacultyId = viewModel.FacultyId,
                    FacultyName = viewModel.FacultyName,
                    Established = viewModel.Established
                };
                _context.Add(faculty);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(viewModel.CoordinatorUserName))
                {
                    var coordinator = await _context.Users.FirstOrDefaultAsync(u => u.UserName == viewModel.CoordinatorUserName);
                    if (coordinator != null)
                    {
                        coordinator.FacultyId = faculty.FacultyId;
                        _context.Update(coordinator);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
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
            ViewBag.CoordinatorUserNames = _context.Users
         .Where(u => u.RoleId == 2 && u.FacultyId == null)
         .Select(u => new SelectListItem
         {
             Text = u.UserName,
             Value = u.UserName
         })
         .ToList();

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
                    var faculty = await _context.Faculties.FindAsync(viewModel.FacultyId);
                    if (faculty == null)
                    {
                        return NotFound();
                    }

                    faculty.FacultyName = viewModel.FacultyName;
                    faculty.Established = viewModel.Established;
                    _context.Update(faculty);

                    var currentCoordinator = await _context.Users
                       .Where(u => u.FacultyId == viewModel.FacultyId && u.Role.RoleName == "Marketing coordinator")
                       .FirstOrDefaultAsync();

                    if (viewModel.CoordinatorUserName != currentCoordinator?.UserName && !string.IsNullOrWhiteSpace(viewModel.CoordinatorUserName))
                    {
                        var newCoordinator = await _context.Users.FirstOrDefaultAsync(u => u.UserName == viewModel.CoordinatorUserName);
                        if (newCoordinator != null && newCoordinator.FacultyId == null)
                        {
                            // Update the old coordinator if exists
                            if (currentCoordinator != null)
                            {
                                currentCoordinator.FacultyId = null; // Unassign the old coordinator
                                _context.Update(currentCoordinator);
                            }
                            newCoordinator.FacultyId = faculty.FacultyId;
                            _context.Update(newCoordinator);
                        }
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
                // Find all users associated with this faculty
                var associatedUsers = _context.Users.Where(u => u.FacultyId == id);

                // Option 1: Set FacultyId to null for all associated users
                foreach (var user in associatedUsers)
                {
                    user.FacultyId = null; // Or set to another valid FacultyId
                }

                // Option 2: Delete all associated users (Use with caution!)
                // _context.Users.RemoveRange(associatedUsers);

                // Save changes for users if needed
                await _context.SaveChangesAsync();

                _context.Faculties.Remove(faculty);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        // POST: Users/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string facultyId)
        {
            var faculty = await _context.Faculties.FindAsync(facultyId);
            if (faculty == null)
            {
                return NotFound();
            }

            // Toggle status
            faculty.Status = (byte)(faculty.Status == 1 ? 0 : 1);

            _context.Update(faculty);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool FacultyExists(string id)
        {
            return (_context.Faculties?.Any(e => e.FacultyId == id)).GetValueOrDefault();
        }
    }
}