// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using JobsBoard.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using JobsBoard.Data;

namespace JobsBoard.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<JobsBoardUser> _signInManager;
        private readonly UserManager<JobsBoardUser> _userManager;
        private readonly IUserStore<JobsBoardUser> _userStore;
        private readonly IUserEmailStore<JobsBoardUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        private readonly JobsBoardContext _Context;
        public RegisterModel(
            UserManager<JobsBoardUser> userManager,
            IUserStore<JobsBoardUser> userStore,
            SignInManager<JobsBoardUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender
            , JobsBoardContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _Context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {

            [Required]
            [DisplayName("أسم المستخدم")]
            public string UserName { get; set; }



            [Required]
            [DisplayName(" الاسم الاول")]
            public string FirstName { get; set; }

            [Required]
            [DisplayName(" الاسم الاخير")]
            public string LastName { get; set; }



            [DisplayName("اختر نوع الحساب")]
            public string UserType { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "البريد الالكتروني")]
            public string Email { get; set; }


            [DisplayName("رقم الجوال ")]
            public string PhoneNumber { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "كلمة السر")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "تأكيد كلمة السر")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }



        }
        public class EditProfileViewModel
        {
            [Required]
            [DisplayName("أسم المستخدم")]
            public string UserName { get; set; }



            [Required]
            [DisplayName(" الاسم الاول")]
            public string FirstName { get; set; }

            [Required]
            [DisplayName(" الاسم الاخير")]
            public string LastName { get; set; }



            [DisplayName("تغير نوع الحساب")]
            public string UserType { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "البريد الالكتروني")]
            public string Email { get; set; }


            [DisplayName("رقم الجوال ")]
            public string PhoneNumber { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = " كلمة السر" )]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "تأكيد كلمة السر")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {

            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("/");
            }
            ListUserType();
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                ListUserType();
                var user = CreateUser();
                var x = user.Id;

                user.PhoneNumber = Input.PhoneNumber;
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.UserType = Input.UserType;
                user.UserName = Input.UserName;
                user.NormalizedUserName = Input.Email;
                var roleId = _Context.Roles
    .Where(r => r.Name == user.UserType)
    .Select(r => r.Id)
    .FirstOrDefault();
                //var role_Id = _Context.Roles.FirstOrDefault(r => r.Name == user.UserType);
                //return Content("Role_id" + roleId);
                //return Content("User id :" + user.Id, Input.UserType);
                //await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);

                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.UserName = Input.UserName;
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                 
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    await _userManager.AddToRoleAsync(user, Input.UserType);
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ListUserType();

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private JobsBoardUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<JobsBoardUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(JobsBoardUser)}'. " +
                    $"Ensure that '{nameof(JobsBoardUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<JobsBoardUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<JobsBoardUser>)_userStore;
        }

        private void ListUserType()
        {
            ViewData["UserType"] = new SelectList(
                _Context.Roles.Where(a => !a.Name.Contains("مدير")).ToList(), "Name", "Name");

        }
    }
}
