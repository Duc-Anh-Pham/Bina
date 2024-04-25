using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace Bina.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "@gmail.com";
            var pw = "ppzvehdvlcfuphws";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(mail);
            mailMessage.Subject = subject;
            mailMessage.To.Add(new MailAddress(email));
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;


            return client.SendMailAsync(mailMessage);
        }
    }
}
