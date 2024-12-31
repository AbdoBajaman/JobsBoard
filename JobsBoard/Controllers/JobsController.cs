using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobsBoard.Data;
using JobsBoard.Models;
using Microsoft.AspNetCore.Identity;
using JobsBoard.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;
using JobsBoard.Service;

namespace JobsBoard.Controllers
{
    //[Authorize(Roles = "صاحب عمل")]

    [Authorize(Roles = "مدير,صاحب عمل")]

    public class JobsController : Controller
    {
        private readonly JobsBoardContext _context;
        private readonly IWebHostEnvironment hosting;

        private readonly UserManager<JobsBoardUser> _userManager;


        private readonly UserService _UserService;

        public JobsController(JobsBoardContext context, IWebHostEnvironment hosting
            , UserManager<JobsBoardUser> userManager,
            UserService UserService)
        {
            _context = context;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

            this.hosting = hosting;
            this._UserService = UserService;


        }

        // GET: Jobs

        public async Task<IActionResult> Index()
        {
            var jobsBoardContext = _context.Job.Include(c => c.Category);
            return View(await jobsBoardContext.ToListAsync());
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
           
            if (id == null || _context.Job == null)
            {
                return NotFound();
            }

            var job = await _context.Job

                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Jobs/Create

        [Authorize(Roles = "صاحب عمل")]
        public async Task<IActionResult> Create()
        {

            var LastId = _context.Job.Max(job => job.Id) + 1;

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName");
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "صاحب عمل")]
        public async Task<IActionResult> Create(Job job, IFormFile? upload)
        {
            var mail = new MailMessage();

            var current_User = await _userManager.GetUserAsync(User);
            var LastId = _context.Job.Max(job => job.Id) ;


            // Set up sender and recipient
            mail.From = new MailAddress(current_User.Email);
            mail.To.Add("bajamanabdo@gmail.com");
            mail.Subject = "طلب الموافقه لاضافه وظيفه جديده";

            // Construct the URL for job details
            var jobDetailsUrl = Url.Action("Details", "Jobs", new { id = LastId + 1 }, protocol: Request.Scheme);

            string body = "اسم المرسل : " + current_User.FirstName + " " + current_User.LastName + "<br>" +
                "بريد المرسل : " + current_User.Email + "<br>" +
                "عنوان الوظيفه : " + job.JobTitle + "<br>" +
                "الوصف : " + job.JobContent + "<br>" +
                "لرؤية تفاصيل الوظيفة، اضغط على الرابط التالي: <a href=\"" + jobDetailsUrl + "\">تفاصيل الوظيفة</a><br>";

            // Set the email body as HTML
            mail.Body = body;
            mail.IsBodyHtml = true;

            // Configure Gmail SMTP server
            var loginInfo = new NetworkCredential("abdo99669@gmail.com", "zpchdhtgovjzzlvi");
            var smtp = new SmtpClient("smtp.gmail.com", 587);

            smtp.Credentials = loginInfo;
            smtp.EnableSsl = true;

            if (ModelState.IsValid)
            {
                string fileName = UploadFile(upload) ?? string.Empty;
                job.JobImage = fileName;
                var UserId = await _userManager.GetUserAsync(User);
                job.UserId = UserId?.Id;
                job.JobStatus = "غير منشور";
                _context.Add(job);
                await _context.SaveChangesAsync();


                smtp.Send(mail);
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", job.CategoryId);
            return View(job);
        }


        // GET: Jobs/Edit/5
        [Authorize(Roles = "صاحب عمل")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Job == null)
            {
                return NotFound();
            }

            var job = await _context.Job.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", job.CategoryId);
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "صاحب عمل")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JobTitle,JobContent,CategoryId,CompanyName")] Job job, IFormFile? upload)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload only if a file is provided
                    if (upload != null && upload.Length > 0)
                    {
                        string fileName = UploadFile(upload); // Custom method to handle file upload
                        job.JobImage = fileName;
                    }
                    else
                    {
                        // Retain the existing image if no new file is uploaded
                        var existingJob = await _context.Job.AsNoTracking().FirstOrDefaultAsync(j => j.Id == job.Id);
                        if (existingJob != null)
                        {
                            job.JobImage = existingJob.JobImage;
                        }
                    }

                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine(string.Join(", ", errors));
            ViewBag.Errors = errors;

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", job.CategoryId);
            return View(job);
        }

        // GET: Jobs/Delete/5
        [Authorize(Roles = "صاحب عمل")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Job == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "صاحب عمل")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Job == null)
            {
                return Problem("Entity set 'JobsBoardContext.Job'  is null.");
            }
            var job = await _context.Job.FindAsync(id);
            if (job != null)
            {
                _context.Job.Remove(job);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        private bool JobExists(int id)
        {
            return (_context.Job?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        string UploadFile(IFormFile file)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, "Images");
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

        string UploadFile(IFormFile file, string imageUrl)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, "Images");

                string newPath = Path.Combine(uploads, file.FileName);
                string oldPath = Path.Combine(uploads, imageUrl);

                if (oldPath != newPath)
                {
                    System.IO.File.Delete(oldPath);
                    file.CopyTo(new FileStream(newPath, FileMode.Create));
                }

                return file.FileName;
            }

            return imageUrl;
        }
    }
}
