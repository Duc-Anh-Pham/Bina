using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Bina.Data;
using Bina.Models.Authentication;

namespace Bina.Controllers
{
    public class ForgotController : Controller
    {
        private readonly Ft1Context _context;

        public ForgotController(Ft1Context context)
        {
            _context = context;
        }

        //[Authentication]
        private string GenerateRandomPassword(int length = 8)
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string numericChars = "0123456789";
            const string specialChars = "@$!%*?&#";

            char[] password = new char[length];
            Random random = new Random();

            // Make sure there is at least one uppercase character
            password[random.Next(length)] = uppercaseChars[random.Next(uppercaseChars.Length)];

            // Make sure there is at least one lowercase character
            password[random.Next(length)] = lowercaseChars[random.Next(lowercaseChars.Length)];

            // Make sure there is at least one number
            password[random.Next(length)] = numericChars[random.Next(numericChars.Length)];

            // Make sure there is at least one special character
            password[random.Next(length)] = specialChars[random.Next(specialChars.Length)];

            // Fill in the remaining characters randomly
            for (int i = 0; i < length; i++)
            {
                if (password[i] == '\0')
                {
                    string validChars = uppercaseChars + lowercaseChars + numericChars + specialChars;
                    password[i] = validChars[random.Next(validChars.Length)];
                }
            }

            return new string(password);
        }


        // GET: Users/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Users/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    // Generate a new random password
                    string newPassword = GenerateRandomPassword();

                    // Update new password for user
                    user.Password = newPassword;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    // Send email with new password
                    SendPasswordResetEmail(user.Email, newPassword);

                    TempData["SuccessMessage"] = "A new password has been sent to your email address.";
                    return RedirectToAction("Login", "Logins");
                }

                ModelState.AddModelError("", "Email not found in the system.");
            }

            return View();
        }

        private void SendPasswordResetEmail(string email, string password)
        {
            string subject = "Your new password";
            string body = $"Your new password is: {password}";

            // Google SMTP information
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "ducanh040202003@gmail.com";
            string smtpPassword = "qeqglgodldcvooki"; 

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                var message = new MailMessage();
                message.From = new MailAddress(smtpUsername);
                message.To.Add(new MailAddress(email));
                message.Subject = subject;
                message.Body = body;

                client.Send(message);
            }
        }

    }
}