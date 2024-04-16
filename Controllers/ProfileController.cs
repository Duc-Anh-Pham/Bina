using Bina.Models.Authentication;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bina.Data;

namespace Bina.Controllers
{
    public class ProfileController : Controller
    {
        private readonly Ft1Context _context;

        public ProfileController(Ft1Context context)
        {
            _context = context;
        }
        // GET: Users/Profile/5
        [Authentication]
        public async Task<IActionResult> EditProfile(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", user.FacultyId);
            ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);

            return View(user);
        }

        // POST: Users/Profile/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int id, [Bind("UserId,FirstName,LastName,PhoneNumber,DoB,Gender,Password")] User user)
        {
            int userId = HttpContext.Session.GetInt32("UserId").Value;
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _context.Users.FindAsync(id);

                    if (userToUpdate == null)
                    {
                        return NotFound();
                    }

                    userToUpdate.FirstName = user.FirstName;
                    userToUpdate.LastName = user.LastName;
                    userToUpdate.PhoneNumber = user.PhoneNumber;
                    userToUpdate.DoB = user.DoB;
                    userToUpdate.Gender = user.Gender;
                    userToUpdate.Password = user.Password;

                    _context.Users.Update(userToUpdate);
                    await _context.SaveChangesAsync();
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

                return RedirectToAction(nameof(Index));
            }

            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", user.FacultyId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
