using Bina.Data;
using Bina.Models;
using Bina.Models.Authentication;
using Bina.Services;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace Bina.Areas.Guest.Controllers
{
    [Area("Guest")]
	public class UsersController : Controller
	{
		private readonly Ft1Context _context;
		private readonly FirebaseCloud _firebaseCloud;

		public UsersController(Ft1Context context, ILogger<UsersController> logger, FirebaseCloud firebaseCloud)
		{
			_context = context;
			_firebaseCloud = firebaseCloud;
		}

		[Authentication]

		// GET: Users/Index/Search
		[HttpGet]
		public async Task<IActionResult> Index(string searchTerm, string sortBy, string sortDirection, int? pageSize = 5, int? pageNumber = 1)
		{
			int defaultPageSize = pageSize ?? 5;
			int currentPageNumber = pageNumber ?? 1;

            // Lấy FacultyId của Guest hiện tại từ session
            string GuestFacultyId = HttpContext.Session.GetString("FacultyId");

            // Lọc danh sách user theo FacultyId của Guest
            IQueryable<User> Ft1Context = _context.Users
                .Include(u => u.Faculty)
                .Include(u => u.Role)
                .Include(u => u.Terms)
                .Where(u => u.FacultyId == GuestFacultyId && u.RoleId != 1);

            //Function Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
			{
                Ft1Context = Ft1Context.Where(u => u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm) || u.FacultyId.Contains(searchTerm));
            }

			// Function Sort alphabetically
			if (!string.IsNullOrWhiteSpace(sortBy))
			{
				var isDesc = string.Equals(sortDirection, "Desc", StringComparison.OrdinalIgnoreCase);

				if (string.Equals(sortBy, "UserName", StringComparison.OrdinalIgnoreCase))
				{
					Ft1Context = isDesc ? Ft1Context.OrderByDescending(u => u.UserName) : Ft1Context.OrderBy(u => u.UserName);
				}
			}

			// Function Pagination
			int totalRecords = await Ft1Context.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalRecords / defaultPageSize);

			// Lấy danh sách users cho trang hiện tại
			var users = await Ft1Context
				.Skip((currentPageNumber - 1) * defaultPageSize)
				.Take(defaultPageSize)
				.ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPageNumber;
            ViewBag.PageSize = defaultPageSize;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
			ViewBag.ErrorMessage = TempData["ErrorMessage"];
			return View(users);
			//return View(await Ft1Context.ToListAsync());
		}

		//// GET: Users/Details/5
		//public async Task<IActionResult> Details(int? id)
		//{
		//	if (id == null || _context.Users == null)
		//	{
		//		return NotFound();
		//	}

		//	var user = await _context.Users
		//	.Include(u => u.Faculty)
		//	.Include(u => u.Role)
		//	.Include(u => u.Terms)
		//	.FirstOrDefaultAsync(m => m.UserId == id);

		//	if (user == null)
		//	{
		//		return NotFound();
		//	}

		//	// Kiểm tra nếu RoleName là "Student"
		//	var role = await _context.Roles.FindAsync(user.RoleId);
		//	ViewBag.IsStudent = role != null && role.RoleName == "Student";

		//	return View(user);
		//}

		//// GET: Users/Create
		//public IActionResult Create()
		//{
  //          // Lấy FacultyId của Guest hiện tại từ session
  //          string GuestFacultyId = HttpContext.Session.GetString("FacultyId");

  //          ViewData["FacultyName"] = new SelectList(_context.Faculties.Where(f => f.FacultyId == GuestFacultyId), "FacultyId", "FacultyName");
  //          var allowedRoles = new List<string> { "Student", "Guest" };
  //          ViewData["RoleName"] = new SelectList(_context.Roles.Where(r => allowedRoles.Contains(r.RoleName)), "RoleId", "RoleName");
  //          ViewData["TermsText"] = new SelectList(_context.TermsAndConditions, "TermsId", "TermsText");

  //          // Lấy RoleName của vai trò "Student"
  //          var studentRoleName = _context.Roles.FirstOrDefault(r => r.RoleName == "Student")?.RoleName;
		//	ViewBag.StudentRoleName = studentRoleName;

		//	return View();
		//}

		//private string GenerateRandomPassword(int length = 8)
		//{
		//	const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		//	const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
		//	const string numericChars = "0123456789";
		//	const string specialChars = "@$!%*?&#";

		//	char[] password = new char[length];
		//	Random random = new Random();

		//	// Make sure there is at least one uppercase character
		//	password[random.Next(length)] = uppercaseChars[random.Next(uppercaseChars.Length)];

		//	// Make sure there is at least one lowercase character
		//	password[random.Next(length)] = lowercaseChars[random.Next(lowercaseChars.Length)];

		//	// Make sure there is at least one number
		//	password[random.Next(length)] = numericChars[random.Next(numericChars.Length)];

		//	// Make sure there is at least one special character
		//	password[random.Next(length)] = specialChars[random.Next(specialChars.Length)];

		//	// Fill in the remaining characters randomly
		//	for (int i = 0; i < length; i++)
		//	{
		//		if (password[i] == '\0')
		//		{
		//			string validChars = uppercaseChars + lowercaseChars + numericChars + specialChars;
		//			password[i] = validChars[random.Next(validChars.Length)];
		//		}
		//	}

		//	return new string(password);
		//}


		//// POST: Users/Create
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> Create([Bind("UserName,FirstName,LastName,PhoneNumber,DoB,DateCreated,Gender,Email,Password,AvatarPath,RoleId,FacultyId,Terms.TermsText")] User user)
		//{
		//	if (ModelState.IsValid)
		//	{
  //              var allowedRoles = new List<string> { "Student", "Guest" };
  //              var selectedRole = await _context.Roles.FindAsync(user.RoleId);

  //              // Lấy FacultyId của Guest hiện tại từ session
  //              string GuestFacultyId = HttpContext.Session.GetString("FacultyId");

  //              // Kiểm tra xem FacultyId của user mới có trùng với FacultyId của Guest hay không
  //              if (user.FacultyId != GuestFacultyId)
  //              {
  //                  ModelState.AddModelError("FacultyId", "User can only be created with the same FacultyId as the current Guest.");
  //                  ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", user.FacultyId);
  //                  ViewData["RoleName"] = new SelectList(_context.Roles.Where(r => allowedRoles.Contains(r.RoleName)), "RoleId", "RoleName", user.RoleId);
  //                  ViewData["TermsText"] = new SelectList(_context.TermsAndConditions, "TermsId", "TermsText", user.TermsId);
  //                  return View(user);
  //              }

  //              if (selectedRole != null && !allowedRoles.Contains(selectedRole.RoleName))
  //              {
  //                  ModelState.AddModelError("RoleId", "Invalid role selected. Only Student or Guest roles are allowed.");
  //                  ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", user.FacultyId);
  //                  ViewData["RoleName"] = new SelectList(_context.Roles.Where(r => allowedRoles.Contains(r.RoleName)), "RoleId", "RoleName", user.RoleId);
  //                  ViewData["TermsText"] = new SelectList(_context.TermsAndConditions, "TermsId", "TermsText", user.TermsId);
  //                  return View(user);
  //              }

  //              // List of allowed email domains
  //              var allowedDomains = new List<string> { "@gmail.com", "@fpt.edu.vn", "@org.com" };

		//		// Check if the email domain is allowed
		//		var emailDomain = user.Email.Substring(user.Email.IndexOf('@')).ToLower(); // Separate the email suffix from the '@' character and convert it to lowercase
		//		if (!allowedDomains.Contains(emailDomain, StringComparer.OrdinalIgnoreCase))
		//		{
		//			ModelState.AddModelError("Email", "Email must be from one of the following domains: @gmail.com, @fpt.edu.vn, @org.com");
		//			ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", user.FacultyId);
		//			ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
		//			ViewData["TermsText"] = new SelectList(_context.TermsAndConditions, "TermsId", "TermsText", user.TermsId);
		//			return View(user);
		//		}

		//		// Check that User Name is not left empty
		//		if (string.IsNullOrWhiteSpace(user.UserName))
		//		{
		//			ModelState.AddModelError("", "User Name cannot be empty!");
		//			ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", user.FacultyId);
		//			ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
		//			return View(user);
		//		}

		//		// Check if email exists or not
		//		var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email || u.UserName == user.UserName);
		//		if (existingUser != null)
		//		{
		//			// Notify that email or User Name already exists
		//			string errorMessage = existingUser.Email == user.Email ? "Email already exists in the system. Please enter another email." : "User Name already exists in the system. Please enter another User Name.";
		//			ModelState.AddModelError(existingUser.Email == user.Email ? "Email" : "UserName", errorMessage);
		//			ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", user.FacultyId);
		//			ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
		//			return View(user);
		//		}

		//		// Assign default avatar image path
		//		user.AvatarPath = "https://firebasestorage.googleapis.com/v0/b/comp1640web.appspot.com/o/avatar%2FAvatar.png?alt=media&token=3f4c73c3-768d-482e-bdc3-f61487b5f35d";

		//		// Generate random passwords
		//		user.Password = GenerateRandomPassword();

		//		// Assign the Role Id value from the form to the user object
		//		user.RoleId = int.Parse(Request.Form["Role"]);

		//		// Create paths
		//		var confirmationToken = GenerateConfirmationToken(user);

		//		// Send user information via email
		//		SendConfirmationEmail(user.Email, confirmationToken, user.Password);

		//		_context.Users.Add(user);
		//		await _context.SaveChangesAsync();

		//		TempData["SuccessMessage"] = "Please confirm your email to activate your account.";
		//		return RedirectToAction(nameof(Index));
		//	}

		//	// (Return statement and view data handling as in your original code)
		//	ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", user.FacultyId);
		//	ViewData["RoleName"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
		//	ViewData["TermsText"] = new SelectList(_context.TermsAndConditions, "TermsId", "TermsText", user.TermsId);
		//	return View(user);
		//}

		//// GET: Users/Edit/5
		//public async Task<IActionResult> Edit(int? id)
		//{
		//	if (id == null || _context.Users == null)
		//	{
		//		return NotFound();
		//	}

		//	var user = await _context.Users
		//	.Include(u => u.Faculty)
		//	.Include(u => u.Role) // Eager loading Roles
		//	.Include(u => u.Terms)
		//	.FirstOrDefaultAsync(m => m.UserId == id);

		//	if (user == null)
		//	{
		//		return NotFound();
		//	}

		//	ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", user.FacultyId);
  //          // Lọc danh sách role chỉ bao gồm "Student" và "Guest"
  //          var allowedRoles = new List<string> { "Student", "Guest" };
  //          ViewData["RoleName"] = new SelectList(_context.Roles.Where(r => allowedRoles.Contains(r.RoleName)), "RoleId", "RoleName", user.RoleId);
  //          ViewData["TermsText"] = new SelectList(_context.TermsAndConditions, "TermsId", "TermsText", user.TermsId);

		//	// Kiểm tra nếu RoleName là "Student"
		//	var role = await _context.Roles.FindAsync(user.RoleId);
		//	ViewBag.IsStudent = role != null && role.RoleName == "Student";

		//	return View(user);
		//}

		//// POST: Users/Edit/5
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,FirstName,LastName,PhoneNumber,DoB,Gender,Email,Password,AvatarPath,RoleId,FacultyId,Terms.TermsText")] User user, IFormFile? AvatarFile)
		//{
		//	if (id != user.UserId)
		//	{
		//		return NotFound();
		//	}

		//	if (ModelState.IsValid)
		//	{
		//		try
		//		{
		//			var userToUpdate = await _context.Users
		//				.Include(u => u.Terms)
		//				.FirstOrDefaultAsync(m => m.UserId == id);

		//			if (userToUpdate == null)
		//			{
		//				return NotFound();
		//			}

		//			// Lưu trữ TermsId hiện tại
		//			var currentTermsId = userToUpdate.TermsId;

		//			// Tạm thời gán TermsId thành null
		//			userToUpdate.TermsId = null;

		//			// Cập nhật các trường dữ liệu
		//			userToUpdate.UserName = user.UserName;
		//			userToUpdate.FirstName = user.FirstName;
		//			userToUpdate.LastName = user.LastName;
		//			userToUpdate.Email = user.Email;
		//			userToUpdate.PhoneNumber = user.PhoneNumber;
		//			userToUpdate.DoB = user.DoB;
		//			userToUpdate.Gender = user.Gender;
		//			userToUpdate.Password = user.Password;
		//			userToUpdate.RoleId = user.RoleId;
		//			userToUpdate.FacultyId = user.FacultyId;

		//			// Xử lý cập nhật AvatarPath
		//			if (AvatarFile != null && AvatarFile.Length > 0)
		//			{
		//				string avatarUrl = await _firebaseCloud.UploadAvatarToFirebase(AvatarFile, $"avatars/{userToUpdate.UserId}");
		//				if (!string.IsNullOrEmpty(avatarUrl))
		//				{
		//					userToUpdate.AvatarPath = avatarUrl;
		//					// Sau khi lưu đường dẫn avatar mới vào cơ sở dữ liệu
		//					HttpContext.Session.SetString("AvatarPath", userToUpdate.AvatarPath);
		//				}
		//			}

		//			// Kiểm tra vai trò mới
		//			var newRole = await _context.Roles.FindAsync(user.RoleId);
		//			if (newRole != null && newRole.RoleName == "Student")
		//			{
		//				// Kiểm tra nếu TermsText không rỗng
		//				if (!string.IsNullOrWhiteSpace(user.Terms?.TermsText))
		//				{
		//					// Tìm TermsAndCondition có TermsText tương ứng trong cơ sở dữ liệu
		//					var existingTerms = await _context.TermsAndConditions
		//						.FirstOrDefaultAsync(t => t.TermsText == user.Terms.TermsText);

		//					if (existingTerms != null)
		//					{
		//						// Nếu TermsText đã tồn tại, gán TermsId hiện có cho user
		//						userToUpdate.TermsId = existingTerms.TermsId;
		//					}
		//					else
		//					{
		//						// Ngược lại, tạo mới TermsAndCondition
		//						var newTerms = new TermsAndCondition { TermsText = user.Terms.TermsText };
		//						_context.TermsAndConditions.Add(newTerms);
		//						await _context.SaveChangesAsync();
		//						userToUpdate.TermsId = newTerms.TermsId;
		//					}
		//				}
		//			}
		//			// Nếu vai trò mới không phải "Student", giữ TermsId là null

		//			_context.Users.Update(userToUpdate);
		//			await _context.SaveChangesAsync();

		//			return RedirectToAction(nameof(Index));
		//		}
		//		catch (DbUpdateConcurrencyException)
		//		{
		//			if (!UserExists(user.UserId))
		//			{
		//				return NotFound();
		//			}
		//			else
		//			{
		//				throw;
		//			}
		//		}
		//	}

		//	ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName", user.FacultyId);
		//	ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
		//	ViewData["TermsText"] = new SelectList(_context.TermsAndConditions, "TermsId", "TermsText", user.TermsId);
		//	return View(user);
		//}

		//// GET: Users/Delete/5
		//public async Task<IActionResult> Delete(int? id)
		//{
		//	if (id == null || _context.Users == null)
		//	{
		//		return NotFound();
		//	}

		//	var user = await _context.Users
		//		.Include(u => u.Faculty)
		//		.Include(u => u.Role)
		//		.Include(u => u.Terms)
		//		.FirstOrDefaultAsync(m => m.UserId == id);
		//	if (user == null)
		//	{
		//		return NotFound();
		//	}

		//	return View(user);
		//}

		//// POST: Users/Delete/5
		//[HttpPost, ActionName("Delete")]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> DeleteConfirmed(int id)
		//{
		//	// Kiểm tra xem người dùng có bài báo đang nộp hay không
		//	var userArticles = _context.Articles.Where(a => a.UserId == id);

		//	if (userArticles.Any())
		//	{
		//		// Nếu người dùng có bài báo đang nộp, hiển thị thông báo lỗi
		//		TempData["ErrorMessage"] = "This user cannot be deleted because they have existing posts.";
		//		return RedirectToAction(nameof(Index));
		//	}

		//	var user = await _context.Users.FindAsync(id);
		//	if (user != null)
		//	{
		//		_context.Users.Remove(user);
		//	}

		//	await _context.SaveChangesAsync();
		//	return RedirectToAction(nameof(Index));
		//}

		//// POST: Users/ToggleStatus/5
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> ToggleStatus(int userId)
		//{
		//	var user = await _context.Users.FindAsync(userId);
		//	if (user == null)
		//	{
		//		return NotFound();
		//	}

		//	// Toggle status
		//	user.Status = (byte)(user.Status == 1 ? 0 : 1);

		//	_context.Update(user);
		//	await _context.SaveChangesAsync();

		//	return RedirectToAction(nameof(Index));
		//}

		[Authentication]
		[HttpGet]
		public async Task<IActionResult> Profile(int? id)
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
		public async Task<IActionResult> Profile(int id, [Bind("UserId,FirstName,LastName,PhoneNumber,DoB,Gender,Password")] User user, IFormFile? AvatarFile)
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

			ViewData["FacultyName"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", user.FacultyId);
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

			//// Kích hoạt tài khoản sau khi đổi mật khẩu thành công
			//existingUser.Status = 1;
			//_context.Users.Update(existingUser);
			//await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "Password changed successfully.";
			return RedirectToAction(nameof(Index));
		}


		//private string GenerateConfirmationToken(User user)
		//{
		//	// Tạo một GUID duy nhất làm token xác nhận
		//	var token = Guid.NewGuid().ToString();

		//	// Tạo đường dẫn xác nhận email
		//	var confirmationLink = $"https://localhost:7234/";

		//	return confirmationLink;
		//}

		//private void SendConfirmationEmail(string email, string confirmationLink, string password)
		//{
		//	string subject = "Your account information";
		//	string body = $"Please click on the link to access the website: {confirmationLink}\n\nYour email: {email}\n\nYour password: {password}";

		//	// Google SMTP information
		//	string smtpServer = "smtp.gmail.com";
		//	int smtpPort = 587;
		//	string smtpUsername = "ducanh040202003@gmail.com";
		//	string smtpPassword = "qeqglgodldcvooki";

		//	using (var client = new SmtpClient(smtpServer, smtpPort))
		//	{
		//		client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
		//		client.EnableSsl = true;

		//		var message = new MailMessage();
		//		message.From = new MailAddress(smtpUsername);
		//		message.To.Add(new MailAddress(email));
		//		message.Subject = subject;
		//		message.Body = body;

		//		client.Send(message);
		//	}
		//}

		//// GET: Users/ConfirmEmail
		//public async Task<IActionResult> ConfirmEmail(int userId, string token, bool changePassword)
		//{
		//    var user = await _context.Users.FindAsync(userId);
		//    if (user == null)
		//    {
		//        // Xử lý trường hợp không tìm thấy user
		//        return NotFound();
		//    }

		//    // Kích hoạt tài khoản user
		//    user.Status = 1;
		//    _context.Users.Update(user);
		//    await _context.SaveChangesAsync();

		//    TempData["SuccessMessage"] = "Your account has been activated successfully.";
		//    return RedirectToAction(nameof(Index));
		//}


		private bool UserExists(int id)
		{
			return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
		}
	}
}