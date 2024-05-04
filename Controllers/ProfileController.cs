using Bina.Models.Authentication;
using Bina.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bina.Data;
using Bina.Services;
using System.Text;
using System.Security.Cryptography;

namespace Bina.Controllers
{
    public class ProfileController : Controller
    {
        private readonly Ft1Context _context;
        private readonly FirebaseCloud _firebaseCloud;

        public ProfileController(Ft1Context context, ILogger<ProfileController> logger, FirebaseCloud firebaseCloud)
        {
            _context = context;
            _firebaseCloud = firebaseCloud;
        }

        // GET: Users/Profile/5
        [Authentication]
        [HttpGet]
        public async Task<IActionResult> EditProfile(int? id)
        {
            int userId = HttpContext.Session.GetInt32("UserId").Value;

            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            if (id != userId)
            {
                TempData["ErrorMessage"] = "You are not authorized to access this page.";
                return RedirectToAction("Profile", new { id = userId });
            }

            var user = await _context.Users
                .Include(u => u.Faculty)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", user.FacultyId);
            ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int id, [Bind("UserId,FirstName,LastName,PhoneNumber,DoB,Gender,Password")] User user, IFormFile? AvatarFile)
        {
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

                    // Cập nhật thông tin người dùng
                    userToUpdate.FirstName = user.FirstName;
                    userToUpdate.LastName = user.LastName;
                    userToUpdate.PhoneNumber = user.PhoneNumber;
                    userToUpdate.DoB = user.DoB;
                    userToUpdate.Gender = user.Gender;
                    // Mã hóa mật khẩu 
                    userToUpdate.NewPassword = user.Password;

                    // Xử lý cập nhật AvatarPath
                    if (AvatarFile != null && AvatarFile.Length > 0)
                    {
                        string avatarUrl = await _firebaseCloud.UploadAvatarToFirebase(AvatarFile, $"avatars/{userToUpdate.UserId}");
                        if (!string.IsNullOrEmpty(avatarUrl))
                        {
                            userToUpdate.AvatarPath = avatarUrl;
                            // Sau khi lưu đường dẫn avatar mới vào cơ sở dữ liệu
                            HttpContext.Session.SetString("AvatarPath", userToUpdate.AvatarPath);
                        }
                    }

                    _context.Users.Update(userToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Profile updated successfully.";
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

            ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", user.FacultyId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
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

            // Mã hóa mật khẩu mới
            string hashedPassword = HashPassword(user.NewPassword);

            // Cập nhật mật khẩu mới (đã được mã hóa) cho người dùng
            existingUser.Password = hashedPassword;

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
