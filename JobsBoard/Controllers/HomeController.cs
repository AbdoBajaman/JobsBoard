using JobsBoard.Areas.Identity.Data;
using JobsBoard.Data;
using JobsBoard.Models;
using JobsBoard.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;


namespace JobsBoard.Controllers
{
    //[Authorize]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<JobsBoardUser> _userManager;

        private readonly JobsBoardContext _context;
        private readonly IWebHostEnvironment hosting;
        //[Authorize]
        public HomeController(ILogger<HomeController> logger, JobsBoardContext context
            , UserManager<JobsBoardUser> User
            , IWebHostEnvironment hosting)
        {
            _context = context;
            _logger = logger;
            _userManager = User;
            this.hosting = hosting;
        }

        public IActionResult Index()
        {
            var jobs = _context.Job.Include(c => c.Category).ToList();
           
            return View(jobs);
        }


        public async Task<IActionResult> Profile()
        {

            JobsBoardUser CurrentUser = await GetCurrentUser();

            return View(CurrentUser);
        }

        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.Job
        .Include(j => j.Category)
        .FirstOrDefaultAsync(j => j.Id == id);
            if (job == null)
            {
                return NotFound();
            }


            HttpContext.Session.SetInt32("JobId", id);


            return View(job);
        }

        [Authorize]
        public IActionResult Contacts()
        {
            return View();
        }
        [Authorize]

        [HttpPost]
        public IActionResult Contact(Contact contact)
        {
            var mail = new MailMessage();

            // Set up sender and recipient
            mail.From = new MailAddress(contact.Email);
            mail.To.Add("bajamanabdo@gmail.com");
            mail.Subject = contact.subject;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CVs", "Abdulrahman Abdullah Bajaman.pdf");

            if (System.IO.File.Exists(filePath))
            {
                var attachment = new Attachment(filePath);
                mail.Attachments.Add(attachment);
            }

            string body = "اسم المرسل : " + contact.Name + "<br>" +
                "بريد المرسل : " + contact.Email + "<br>" +
                "عنوان الرساله : " + contact.subject + "<br>" +
                "نص الرساله : " + contact.Message + "<br>";

            // Set the email body as HTML
            mail.Body = body;
            mail.IsBodyHtml = true;

            // Configure Gmail SMTP server
            var loginInfo = new NetworkCredential("abdo99669@gmail.com", "zpchdhtgovjzzlvi");
            var smtp = new SmtpClient("smtp.gmail.com", 587);


            smtp.Credentials = loginInfo;
            smtp.EnableSsl = true; 
            smtp.Send(mail);


            return RedirectToAction("Contacts");
        }

        [Authorize(Roles = "باحث عن عمل")]

        public IActionResult Applay()
        {
            return View();
        }
        [Authorize(Roles = "باحث عن عمل")]

        [HttpPost]
        public async Task<IActionResult> Applay(string message, IFormFile upload)
        {
            JobsBoardUser currentUser = await _userManager.GetUserAsync(User);


            string path = UploadFile(upload);


            var submiting = SumbitToJob(message, currentUser, path);
            if (submiting.Message != null)
            {
                _context.ApplyForJob.Add(submiting);

                await SaveChange();
                TempData["Result"] = "تم التقديم بنجاح";
            }
            else
            {
                TempData["Result"] = "لقد تم التقديم مسبقاً في هذه الوظيفه!!";
            }


            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [Authorize(Roles = "باحث عن عمل")]


        public async Task<JobsBoardUser> GetCurrentUser()
        {
            JobsBoardUser UserId = await _userManager.GetUserAsync(User);
            return UserId;

        }
        public async Task<IActionResult> GetJobByUser()
        {
            JobsBoardUser UserId = await _userManager.GetUserAsync(User);
            if (UserId == null)
            { return NotFound(); }
            var Jobs = _context.ApplyForJob?.Where(a => a.UserId == UserId.Id).ToList();



            return View(Jobs);
        }


        public async Task<IActionResult> GetJobsByPublisher()
        {
            var user = await _userManager.GetUserAsync(User);

            // Eagerly load Job and User, and materialize the query
            var jobs = await _context.ApplyForJob
                .Include(app => app.Job)
                .Include(app => app.User)
                .Where(app => app.Job.UserId == user.Id)
                .ToListAsync();

            // Group jobs by JobTitle and create a JobViewModel
            var groupedInfo = jobs
                .GroupBy(j => j.Job.JobTitle)
                .Select(g => new JobViewModel
                {
                    JobTitle = g.Key,
                    ApplyForJob = g.ToList() // Convert group to list
                })
                .ToList();

            return View(groupedInfo);
        }

        public IActionResult ApplayDetails(int id)
        {
            // Fetch the job application details, including the related job
            var jobDetails = _context.ApplyForJob
                                     .Include(c => c.Job)
                                     .FirstOrDefault(a => a.Id == id);

            // Check if no job details are found
            if (jobDetails == null)
            {
                return NotFound();
            }

            //// If CV path exists, process file size
            //if (!string.IsNullOrEmpty(jobDetails.CVPath))
            //{
            //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CVs", jobDetails.CVPath);
            //    var fileInfo = new FileInfo(filePath);

            //    // Check if the file exists before trying to access its properties

            //        // Format the file size using the helper method
            //        var formattedSize = FormatFileSize(fileInfo.Length);

            //        // Pass the formatted file size to the view
            //        ViewData["FormattedFileSize"] = formattedSize;

            //}

            return View(jobDetails);
        }

        public string FormatFileSize(long bytes)
        {
            const int kb = 1024;
            const int mb = kb * 1024;

            if (bytes >= mb)
            {
                return $"{(bytes / (float)mb):F2} MB";
            }
            else if (bytes >= kb)
            {
                return $"{(bytes / (float)kb):F2} KB";
            }
            else
            {
                return $"{bytes} bytes";
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ApplyForJob SumbitToJob(string message, JobsBoardUser currentUser, string path)
        {
            var jobId = HttpContext.Session.GetInt32("JobId");
            var check = _context.ApplyForJob.Where(a => a.JobId == jobId && a.UserId == currentUser.Id).ToList();
            ApplyForJob Applay = new ApplyForJob();
            if (check.Count < 1)
            {

                Applay.ApplayDate = DateTime.Now;
                Applay.JobId = (int)jobId;
                Applay.UserId = currentUser.Id;
                Applay.Message = message;
                Applay.CVPath = path;



            }

            return Applay;


        }

        private async Task SaveChange()
        {
            await _context.SaveChangesAsync();
        }

        string UploadFile(IFormFile file)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, "CVs");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                string fullPath = Path.Combine(uploads, file.FileName);
                file.CopyTo(new FileStream(fullPath, FileMode.Create));

                return file.FileName;
            }

            return null;
        }


        public async Task<IActionResult> Search(string searchname)
        {

            var categories = await _context.Category
                .Include(c => c.Jobs)
                .Where(c => c.CategoryName.Contains(searchname)
                        || c.CategoryDescription.Contains(searchname)
                        || c.Jobs.Any(j => j.JobContent.Contains(searchname) || j.JobTitle.Contains(searchname)))
                .ToListAsync();

            return Json(categories);
        }




    }
}