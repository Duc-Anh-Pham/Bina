using Bina.Data;
using Bina.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

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
                        Password = parts[1]
                    };
                    return View(user);
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var u = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(us => us.Email.Equals(user.Email) && us.Password.Equals(user.Password));

            if (u != null)
            {
                if (user.RememberMe)
                {
                    // Lưu thông tin đăng nhập vào cookie
                    var emailAndPassword = $"{user.Email}:{user.Password}";
                    var encryptedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailAndPassword));
                    Response.Cookies.Append("RememberMe", encryptedData, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(30) // Đặt thời gian hết hạn của cookie (ví dụ: 30 ngày)
                    });
                }

                // Save the changes to the database
                _context.SaveChanges();
                if (u.FacultyId != null)
                    HttpContext.Session.SetString("FacultyId", u.FacultyId);

                HttpContext.Session.SetString("Email", u.Email.ToString());
                HttpContext.Session.SetString("UserName", u.UserName.ToString());
                HttpContext.Session.SetInt32("RoleId", u.RoleId.Value);
                HttpContext.Session.SetInt32("UserId", u.UserId);

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
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

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
            if (user.FacultyId != null)
                HttpContext.Session.SetString("FacultyId", user.FacultyId);

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
                TempData["ErrorMessage"] = "Tài khoản không khả dụng!";
                return RedirectToAction("Login", "Logins");
            }

            // Lưu thông tin người dùng vào session
            HttpContext.Session.SetString("UserName", user.UserName.ToString());
            HttpContext.Session.SetString("Email", user.Email.ToString());
            HttpContext.Session.SetInt32("RoleId", user.RoleId.Value);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            if (user.FacultyId != null)
                HttpContext.Session.SetString("FacultyId", user.FacultyId);
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