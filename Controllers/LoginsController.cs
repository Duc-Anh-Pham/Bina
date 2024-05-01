using Microsoft.AspNetCore.Mvc;
using Bina.Models;
using Bina.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Bina.Controllers
{
    public class LoginsController : Controller
	{
		private readonly Ft1Context _context;

		public LoginsController(Ft1Context db)
		{
			_context = db;
		}

		[HttpGet]
		public IActionResult Login()
		{
            // Xóa phiên làm việc hiện tại
            HttpContext.Session.Clear();

            // Kiểm tra xem có thông tin đăng nhập được lưu trong cookie hay không
            var rememberMeCookie = Request.Cookies["RememberMe"];
            if (rememberMeCookie != null)
            {
                var emailAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(rememberMeCookie));
				var parts = emailAndPassword.Split(':');
				if (parts.Length == 2)
				{
					var user = new User
					{
						Email = parts[0],
						Password = parts[1],
						RememberMe = true
					};
					return View(user);
				}
			}
            else
            {
                // Xóa cookie "RememberMe" nếu nó tồn tại
                Response.Cookies.Delete("RememberMe");
            }

            return View();
		}

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var u = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(us => us.Email.Equals(user.Email) && HashPassword(user.Password).Equals(us.Password) || us.Password.Equals(user.Password));

            if (u != null)
            {
                if (u.Status == 0)
                {
                    // Nếu tài khoản bị vô hiệu hóa, lưu thông báo vào TempData và quay lại view Login
                    TempData["ErrorMessage"] = "Your account is disabled.";
                    return View(user);
                }

                if (user.RememberMe)
                {
                    // Lưu thông tin đăng nhập vào cookie
                    var emailAndPassword = $"{user.Email}:{user.Password}";
                    var encryptedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailAndPassword));
                    Response.Cookies.Append("RememberMe", encryptedData, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(30)
                    });
                }
                else
                {
                    // Xóa cookie "RememberMe" nếu nó tồn tại
                    Response.Cookies.Delete("RememberMe");
                }

                // Save the changes to the database
                _context.SaveChanges();

                HttpContext.Session.SetString("Email", u.Email.ToString());
                HttpContext.Session.SetString("UserName", u.UserName.ToString());
                HttpContext.Session.SetInt32("RoleId", u.RoleId.Value);
                HttpContext.Session.SetInt32("UserId", u.UserId);

                if (u.RoleId == 2 && u.RoleId == 3 || u.FacultyId != null)
                {
                    HttpContext.Session.SetString("FacultyId", u.FacultyId.ToString());
                }

                // Check if u.AvatarPath is not null before setting it to the session
                if (u.AvatarPath != null)
                {
                    HttpContext.Session.SetString("AvatarPath", u.AvatarPath.ToString());
                }
                else
                {
                    // Set a default avatar path if u.AvatarPath is null
                    HttpContext.Session.SetString("AvatarPath", "https://firebasestorage.googleapis.com/v0/b/comp1640web.appspot.com/o/avatar%2FAvatar.png?alt=media&token=3f4c73c3-768d-482e-bdc3-f61487b5f35d");
                }

                // Kiểm tra RoleId và chuyển hướng đến Area tương ứng
                switch (u.Role.RoleId)
                {
                    case 1: // Admin
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    case 2: // Coordinator
                        return RedirectToAction("Index", "Home", new { area = "Coordinator" });
                    case 3: // Manager
                        return RedirectToAction("Index", "Home", new { area = "Manager" });
                    case 4: // Students
                        return RedirectToAction("Index", "Home");
					default:
						return RedirectToAction("Index", "Home", new { area = "Guest" });
                }
            }
            else
            {
                // Nếu không tìm thấy người dùng, lưu thông báo lỗi vào TempData và quay lại view Login
                TempData["ErrorMessage"] = "Invalid Email or Password";
                return View(user);
            }
        }

        public async Task Google()
		{
			await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
				new AuthenticationProperties()
				{
					RedirectUri = Url.Action("GoogleResponse")
				});
		}

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Lấy thông tin từ Google
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
            var avatarUrl = claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            // Kiểm tra xem người dùng đã tồn tại trong database hay chưa
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                // Nếu người dùng không tồn tại, trả về trang Login với thông báo lỗi
                TempData["ErrorMessage"] = "Unavailable!";
                return RedirectToAction("Login", "Logins");
            }

            // Lưu thông tin người dùng vào session
            HttpContext.Session.SetString("UserName", user.UserName.ToString());
            HttpContext.Session.SetString("Email", user.Email.ToString());
            HttpContext.Session.SetInt32("RoleId", user.RoleId.Value);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            if (user.RoleId == 2 && user.RoleId == 3 || user.FacultyId != null)
            {
                HttpContext.Session.SetString("FacultyId", user.FacultyId.ToString());
            }
            // Lưu avatar URL vào database
            if (avatarUrl != null)
            {
                var identity = User.Identity as ClaimsIdentity;
                identity.AddClaim(new Claim("AvatarPath", avatarUrl));
                user.AvatarPath = avatarUrl;
                _context.Update(user);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("AvatarPath", avatarUrl);
            }

            // Kiểm tra RoleId và chuyển hướng đến Area tương ứng
            return RedirectToAreaBasedOnRoleId(user.RoleId.Value);
        }


        public async Task Microsoft()
		{
			await HttpContext.ChallengeAsync(MicrosoftAccountDefaults.AuthenticationScheme,
				new AuthenticationProperties()
				{
					RedirectUri = Url.Action("MicrosoftResponse")
				});
		}

		public async Task<IActionResult> MicrosoftResponse()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			// Lấy thông tin từ Microsoft
			var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
			var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
			var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

			// Kiểm tra xem người dùng đã tồn tại trong database hay chưa
			var user = _context.Users.FirstOrDefault(u => u.Email == email);

			if (user == null)
			{
				// Nếu người dùng không tồn tại, trả về trang Login với thông báo lỗi
				TempData["ErrorMessage"] = "Unavailable!";
				return RedirectToAction("Login", "Logins");
			}

            // Lưu thông tin người dùng vào session
            HttpContext.Session.SetString("UserName", user.UserName.ToString());
            HttpContext.Session.SetString("Email", user.Email.ToString());
            HttpContext.Session.SetInt32("RoleId", user.RoleId.Value);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            if (user.RoleId == 2 && user.RoleId == 3 || user.FacultyId != null)
            {
                HttpContext.Session.SetString("FacultyId", user.FacultyId.ToString());
            }

            // Check if u.AvatarPath is not null before setting it to the session
            if (user.AvatarPath != null)
            {
                HttpContext.Session.SetString("AvatarPath", user.AvatarPath.ToString());
            }

            // Kiểm tra RoleId và chuyển hướng đến Area tương ứng
            return RedirectToAreaBasedOnRoleId(user.RoleId.Value);
		}

		private IActionResult RedirectToAreaBasedOnRoleId(int roleId)
		{
			switch (roleId)
			{
                case 1: // Admin
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                case 2: // Coordinator
                    return RedirectToAction("Index", "Home", new { area = "Coordinator" });
                case 3: // Manager
                    return RedirectToAction("Index", "Home", new { area = "Manager" });
                case 4: // Students
                    return RedirectToAction("Index", "Home");
                default:
                    return RedirectToAction("Index", "Home", new { area = "Guest" });
            }
		}

		//create forgot password 

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			await HttpContext.SignOutAsync();

			HttpContext.Session.Clear();
			HttpContext.Session.Remove("Email");

			return RedirectToAction("Index", "Home");
		}
	}
}