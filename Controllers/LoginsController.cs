using Microsoft.AspNetCore.Mvc;
using Bina.Models;
using Bina.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			HttpContext.Session.Remove("Email");
			return RedirectToAction("Login", "Logins");
		}
	}
}