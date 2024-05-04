using Bina.Data;
using Bina.Models;
using Bina.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Cms;
using System.Net.Mail;
using System.Net;

namespace Bina.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly Ft1Context _context;
        private readonly FirebaseCloud _firebaseCloud;
        private readonly IWebHostEnvironment _env;

        public ArticlesController(Ft1Context context, ILogger<ArticlesController> logger, FirebaseCloud firebaseCloud, IWebHostEnvironment env)
        {
            _context = context;
            _firebaseCloud = firebaseCloud;
            _env = env;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Logins");
            }

            var articles = _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.Faculty)
                .Include(a => a.User)
                .Where(a => a.UserId == userId);

            return View(await articles.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.Faculty)
                .Include(a => a.User)
                .Include(a => a.CommentFeedbacks)
                            .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedback(int? articleId, string feedbackText)
        {
            if (!ModelState.IsValid || articleId == null || string.IsNullOrWhiteSpace(feedbackText))
            {
                articleId = 1;
                feedbackText = "No feedback";
            }

            var userId = GetCurrentUserIdFromSession();

            var newFeedback = new CommentFeedback
            {
                CommentFeedbackId = Guid.NewGuid(),
                ArticleId = articleId.Value,

                UserId = userId.Value,
                ContentFeedback = feedbackText,
                CommentDay = DateTime.Now
            };

            _context.CommentFeedbacks.Add(newFeedback);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }


            return RedirectToAction("Details", "Articles", new { id = articleId.Value });
        }
        private int? GetCurrentUserIdFromSession()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId;
        }

        // GET: Articles/Create
        public IActionResult Create()
        {

            // Get UserId from session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // If UserId is not found, possibly user is not logged in
                return RedirectToAction("Login", "Logins");
            }

            var user = _context.Users
            .Include(u => u.Faculty)
            .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.FacultyId == null)
            {

                // If user or user's faculty is not found
                return RedirectToAction("ErrorPage", new { message = "Your faculty information is missing." });
            }

            // Get the current deadline for the user's faculty
            var currentDeadline = _context.ArticlesDeadlines
                .FirstOrDefault(ad => ad.FacultyId == user.FacultyId && ad.DueDate >= DateTime.Now);

            if (currentDeadline == null)
            {
                // If current date is past the DueDate or there's no active deadline
                return RedirectToAction("ErrorPage", new { message = "Currently, this Department has closed the Article Submission function. Please contact the Coordinator if you need further assistance" });
            }

            var deadlines = _context.ArticlesDeadlines
               .Where(a => a.FacultyId == user.FacultyId)
               .OrderBy(a => a.DueDate)
               .ToList();

            var faculties = _context.Faculties
          .Where(f => f.FacultyId == user.FacultyId)
          .ToList();

            // Get the current deadlines for the user's faculty that are still valid
            var validDeadlines = _context.ArticlesDeadlines
                .Where(ad => ad.FacultyId == user.FacultyId && ad.DueDate >= DateTime.Now)
                .OrderBy(ad => ad.DueDate)
                .ToList();

            if (!validDeadlines.Any())
            {
                // If there are no valid deadlines
                return RedirectToAction("ErrorPage", new { message = "Currently, this Department has closed the Article Submission function. Please contact the Coordinator if you need further assistance" });
            }

            // Create a SelectList for the current valid deadlines
            var deadlineTermsSelectList = validDeadlines.Select(ad => new SelectListItem
            {
                Value = ad.ArticlesDeadlineId.ToString(),
                Text = $"{ad.TermTitle} - From {ad.StartDue?.ToString("dd/MM/yyyy")} => To {ad.DueDate?.ToString("HH:mm MMMM dd, yyyy")}"
            }).ToList();


            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId");
            ViewData["ArticlesDeadlineId"] = new SelectList(deadlines, "ArticlesDeadlineId", "TermTitle");
            ViewData["FacultyId"] = new SelectList(faculties, "FacultyId", "FacultyId", user.FacultyId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", userId);
            ViewData["DeadlineTerms"] = deadlineTermsSelectList;

            var article = new Article
            {
                UserId = userId,
                ArticleStatusId = 3,
                FacultyId = user.FacultyId, // Đặt khoa mặc định cho bài viết dựa trên khoa của người dùng
                ArticlesDeadlineId = validDeadlines.First().ArticlesDeadlineId
            };

            // Lấy deadline đầu tiên phù hợp hoặc null nếu không có
            /* article.ArticlesDeadlineId = deadlines.FirstOrDefault()?.ArticlesDeadlineId;*/


            return View(article);
        }

        // POST: Articles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article, IFormFile imageFile, IFormFile documentFile)
        {
            if (ModelState.IsValid)
            {
                article.GuestAllow = false;
                article.DateCreate = DateTime.Now;
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageUrl = await _firebaseCloud.UploadFileToFirebase(imageFile);
                    article.ImagePath = imageUrl;
                }

                // Xử lý file tài liệu
                if (documentFile != null && documentFile.Length > 0)
                {
                    var documentUrl = await _firebaseCloud.UploadFileToFirebase(documentFile);
                    article.DocumentPath = documentUrl;
                }

                _context.Add(article);
                await _context.SaveChangesAsync();

                //send mail for all coor
                int? userId = HttpContext.Session.GetInt32("UserId");
                var user = _context.Users
                .Include(u => u.Faculty)
                .FirstOrDefault(u => u.UserId == userId);
                var allCoor = _context.Users.Where(c => c.RoleId == 2 && c.FacultyId == article.FacultyId).ToList();

                var pathToFile = _env.WebRootPath
             + Path.DirectorySeparatorChar.ToString()
             + "Templates"
             + Path.DirectorySeparatorChar.ToString()
             + "EmailTemplate"
             + Path.DirectorySeparatorChar.ToString()
             + "Confirm_Articles.html";

                var subject = "Confirm Articles";
                string HtmlBody = "";
                using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
                {
                    HtmlBody = streamReader.ReadToEnd();
                }
                //{0}: Subject
                //{1}: DateTime
                //{2}: ArticleName
                //{3}: Email
                //{4}: Image
                //{5}: Faculty
                //{6}: Message
                //{7}: callBackURL
                string Message = $"Please confirm your account ";
                string messageBody = string.Format(HtmlBody,
                       subject,
                       String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
                       article.ArticleName,
                       user?.Email,
                       article.FacultyId,
                       Message
                       );

                try
                {
                    // Configure SMTP client
                    using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                    {
                        smtpClient.Port = 587; // SMTP port (e.g., 587 for TLS)
                        smtpClient.Credentials = new NetworkCredential("anunicore@gmail.com", "rmvc jzxm nwyq jqpb");
                        smtpClient.EnableSsl = true; // Enable SSL/TLS

                        // Create email message
                        using (var message = new MailMessage())
                        {
                            message.From = new MailAddress("anunicore@gmail.com");
                            message.To.Add(user.Email);
                            message.Subject = "Confirm Articles";
                            // Create HTML view
                            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(messageBody, null, "text/html");
                            message.AlternateViews.Add(htmlView);

                            // Send email
                            smtpClient.Send(message);
                        }

                        foreach (User recipient in allCoor)
                        {
                            // Create email message
                            using (var message = new MailMessage())
                            {
                                message.From = new MailAddress("anunicore@gmail.com");
                                message.To.Add(recipient.Email);
                                message.Subject = "Confirm Articles";
                                // Create HTML view
                                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(messageBody, null, "text/html");
                                message.AlternateViews.Add(htmlView);

                                // Send email
                                smtpClient.Send(message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(article);
        }


        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId", article.ArticleStatusId);
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "ArticlesDeadlineId", article.ArticlesDeadlineId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", article.FacultyId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", article.UserId);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,ArticleName,Title,Content,UserId,ImagePath,DocumentPath,ArticleStatusId,ArticlesDeadlineId,FacultyId")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
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
            ViewData["ArticleStatusId"] = new SelectList(_context.ArticleStatuses, "ArticleStatusId", "ArticleStatusId", article.ArticleStatusId);
            ViewData["ArticlesDeadlineId"] = new SelectList(_context.ArticlesDeadlines, "ArticlesDeadlineId", "ArticlesDeadlineId", article.ArticlesDeadlineId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", article.FacultyId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", article.UserId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.ArticleStatus)
                .Include(a => a.ArticlesDeadline)
                .Include(a => a.Faculty)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }

        // Custom error page action
        public IActionResult ErrorPage(string message)
        {
            ViewData["ErrorMessage"] = message;
            return View();
        }
    }
}
