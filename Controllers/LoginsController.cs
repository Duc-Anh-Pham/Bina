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
			if (HttpContext.Session.GetString("Email") == null)
			{
				return View();
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		[HttpPost]
		public IActionResult Login(User user)
		{
			var u = _context.Users
				.Include(u => u.Role)
				.FirstOrDefault(us => us.Email.Equals(user.Email) && us.Password.Equals(user.Password));

			if (u != null)
			{
				HttpContext.Session.SetString("Email", u.Email.ToString());
				HttpContext.Session.SetInt32("RoleId", u.RoleId.Value);

				// Kiểm tra RoleId và chuyển hướng đến Area tương ứng
				switch (u.Role.RoleId)
				{
					case 1: // Admin
						return RedirectToAction("Index", "Home", new { area = "Admin" });
					case 2: // Coordinator
                        return RedirectToAction("Index", "Home", new { area = "Coordinator" });
					case 3: // Manager
                        return RedirectToAction("Index", "Home", new { area = "Manager" });
					default: // Students
						return RedirectToAction("Index", "Home");
				}
			}

			// Nếu không tìm thấy người dùng, chúng ta sẽ trả về View đăng nhập
			return View();
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
			var claims = result.Principal.Identities.FirstOrDefault().Claims;
			var email = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
			var name = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

			// Kiểm tra xem người dùng đã tồn tại trong database hay chưa
			var user = _context.Users.FirstOrDefault(u => u.Email == email);

			if (user == null)
			{
				// Tạo người dùng mới
				user = new User
				{
					Email = email,
					UserFullName = name,
					RoleId = 4 // Assuming default is RoleId = 4 (Student)
				};
				_context.Users.Add(user);
			}
			else
			{
				// Cập nhật thông tin nếu cần thiết
				user.UserFullName = name;
			}

			// Lưu thay đổi vào database
			_context.SaveChanges();

			// Lưu thông tin người dùng vào session
			HttpContext.Session.SetString("Email", user.Email.ToString());
			HttpContext.Session.SetInt32("RoleId", user.RoleId.Value);

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

			// Extract the claims from the Microsoft account
			var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
			var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
			var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

			// Check if the user exists in the database
			var user = _context.Users.FirstOrDefault(u => u.Email == email);

			if (user == null)
			{
				// Create a new user if they don't exist
				user = new User
				{
					Email = email,
					UserFullName = name,
					RoleId = 4 // Assuming default is RoleId = 4 (Student)
				};
				_context.Users.Add(user);
			}
			else
			{
				// Update the user's name if necessary
				user.UserFullName = name;
			}

			// Save the changes in the database
			_context.SaveChanges();

			// Lưu thông tin người dùng vào session
			HttpContext.Session.SetString("Email", user.Email.ToString());
			HttpContext.Session.SetInt32("RoleId", user.RoleId.Value);

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
				default: // Students
					return RedirectToAction("Index", "Home");
			}
		}
	
	public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			HttpContext.Session.Remove("Email");
			return RedirectToAction("Login", "Logins");
		}
	}
}