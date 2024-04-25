using Bina.Data;
using Bina.Models;
using Bina.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bina.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly Ft1Context _context;

        public UsersController(Ft1Context context)
        {
            _context = context;
        }

        [Authentication]

        // GET: Users/Index/Search
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            IQueryable<User> Ft1Context = _context.Users
                .Include(u => u.Faculty)
                .Include(u => u.Role)
                .Include(u => u.Terms);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Ft1Context = Ft1Context.Where(u => u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(await Ft1Context.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
            .Include(u => u.Faculty)
            .Include(u => u.Role)
            .Include(u => u.Terms)
            .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(user.RoleId);
            ViewBag.IsStudent = role != null && role.RoleName == "Student";

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId");
            ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            ViewData["TermsText"] = new SelectList(_context.TermsAndConditions, "TermsId", "TermsText");

            var studentRoleName = _context.Roles.FirstOrDefault(r => r.RoleName == "Student")?.RoleName;
            ViewBag.StudentRoleName = studentRoleName;

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,FirstName,LastName,PhoneNumber,DoB,DateCreated,Gender,Email,Password,RoleId,FacultyId,Terms.TermsText")] User user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
                {
                    ModelState.AddModelError("", "Không được bỏ trống!");
                    ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", user.FacultyId);
                    ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
                    return View(user);
                }


                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email || u.UserName == user.UserName);
                if (existingUser != null)
                {

                    string errorMessage = existingUser.Email == user.Email ? "Email đã tồn tại trong hệ thống. Vui lòng nhập email khác." : "UserName đã tồn tại trong hệ thống. Vui lòng nhập UserName khác.";
                    ModelState.AddModelError(existingUser.Email == user.Email ? "Email" : "UserName", errorMessage);
                    ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", user.FacultyId);
                    ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
                    return View(user);
                }

                user.RoleId = int.Parse(Request.Form["Role"]);

                //var roleId = user.RoleId;
                //var role = await _context.Roles.FindAsync(roleId);
                //if (role != null && role.RoleName == "Student")
                //{
                //    user.Terms = new TermsAndCondition { TermsText = ViewBag.TermsText };
                //}

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm người dùng thành công!"; // Add this line
                return RedirectToAction(nameof(Index));
            }

            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", user.FacultyId);
            ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            ViewData["TermsText"] = new SelectList(_context.TermsAndConditions, "TermsId", "TermsText", user.TermsId);

            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Faculty)
                .Include(u => u.Role)
                .Include(u => u.Terms)
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", user.FacultyId);
            ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId); // Tạo SelectList cho Roles
            ViewData["TermsText"] = new SelectList(_context.TermsAndConditions, "TermsId", "TermsText", user.TermsId);


            var role = await _context.Roles.FindAsync(user.RoleId);
            ViewBag.IsStudent = role != null && role.RoleName == "Student";

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,FirstName,LastName,PhoneNumber,DoB,DateCreated,Gender,Email,Password,RoleId,FacultyId,Terms")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _context.Users
                        .Include(u => u.Terms)
                        .FirstOrDefaultAsync(m => m.UserId == id);

                    if (userToUpdate == null)
                    {
                        return NotFound();
                    }


                    var currentTermsId = userToUpdate.TermsId;


                    userToUpdate.TermsId = null;

                    userToUpdate.UserName = user.UserName;
                    userToUpdate.FirstName = user.FirstName;
                    userToUpdate.LastName = user.LastName;
                    userToUpdate.Email = user.Email;
                    userToUpdate.PhoneNumber = user.PhoneNumber;
                    userToUpdate.DoB = user.DoB;
                    userToUpdate.Gender = user.Gender;
                    userToUpdate.Password = user.Password;
                    userToUpdate.RoleId = user.RoleId;
                    userToUpdate.FacultyId = user.FacultyId;

                    _context.Users.Update(userToUpdate);
                    await _context.SaveChangesAsync();


                    var newRole = await _context.Roles.FindAsync(user.RoleId);
                    if (newRole != null && newRole.RoleName == "Student")
                    {

                        if (!string.IsNullOrWhiteSpace(user.Terms?.TermsText))
                        {

                            var existingTerms = await _context.TermsAndConditions
                                .FirstOrDefaultAsync(t => t.TermsText == user.Terms.TermsText);

                            if (existingTerms != null)
                            {

                                userToUpdate.TermsId = existingTerms.TermsId;
                            }
                            else
                            {

                                var newTerms = new TermsAndCondition { TermsText = user.Terms.TermsText };
                                _context.TermsAndConditions.Add(newTerms);
                                await _context.SaveChangesAsync();
                                userToUpdate.TermsId = newTerms.TermsId;
                            }
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", user.FacultyId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Faculty)
                .Include(u => u.Role)
                .Include(u => u.Terms)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Ft1Context.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Users/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Toggle status
            user.Status = (byte)(user.Status == 1 ? 0 : 1);

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
