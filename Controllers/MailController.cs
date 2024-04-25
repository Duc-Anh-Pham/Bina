using Bina.Models;
using Bina.Services;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Pkcs;

namespace Bina.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailController : ControllerBase
    {
        private readonly Ft1Context _context;
        private readonly IEmailSender _mailService;
        private readonly IWebHostEnvironment _env;
        //injecting the IMailService into the constructor
        public MailController(Ft1Context context, IEmailSender _MailService, IWebHostEnvironment env)
        {
            _context = context;
            _mailService = _MailService;
            _env = env;
        }

        [HttpPost]
        [Route("SendMail")]
        public async Task SendMail(string email, string subject, string message)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users
            .Include(u => u.Faculty)
            .FirstOrDefault(u => u.UserId == userId);
            var pathToFile = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplate"
                            + Path.DirectorySeparatorChar.ToString()
                            + "Confirm_Articles.html";

            string HtmlBody = "";
            using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            {
                HtmlBody = streamReader.ReadToEnd();
            }
            //{0}: Subject
            //{1}: DateTime
            //{2}: ArticleName
            //{3}: Email
            //{4}: Faculty
            //{5}: Message
            //{6}: callBackURL
            string Message = $"Please confirm your account ";
            string messageBody = string.Format(HtmlBody,
                    subject,
                    String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
                    email,
                    "abc@gmail.com",
                    "FIT",
                    Message
                    );
            await _mailService.SendEmailAsync(email, subject, messageBody);

        }
    }
}
