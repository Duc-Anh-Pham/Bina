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
            int userId = HttpContext.Session.GetInt32("UserId").Value;

            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            // Kiểm tra xem id có khớp với userId của người dùng đăng nhập hay không
            if (id != userId)
            {
                TempData["ErrorMessage"] = "You are not authorized to access this page.";
                return RedirectToAction("EditProfile", new { id = userId });
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

        // GET: Users/ChangePassword/5
        [HttpGet]
        public async Task<IActionResult> ChangePassword(int? id)
        {
            int userId = HttpContext.Session.GetInt32("UserId").Value;

            if (id == null)
            {
                return NotFound();
            }

            // Kiểm tra xem id có khớp với userId của người dùng đăng nhập hay không
            if (id != userId)
            {
                TempData["ErrorMessage"] = "You are not authorized to access this page.";
                return RedirectToAction("ChangePassword", new { id = userId });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/ChangePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int id, [Bind("UserId,OldPassword,NewPassword,ConfirmPassword")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            // Kiểm tra mật khẩu cũ có đúng không
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser.Password != user.OldPassword)
            {
                ModelState.AddModelError("OldPassword", "The old password is incorrect.");
                return View(user);
            }

            // Kiểm tra mật khẩu mới và xác nhận mật khẩu mới có khớp không
            if (user.NewPassword != user.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "New password and confirm new password do not match.");
                return View(user);
            }

            // Cập nhật mật khẩu mới cho người dùng
            existingUser.Password = user.NewPassword;
            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
