// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using JobsBoard.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobsBoard.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<JobsBoardUser> _userManager;
        private readonly SignInManager<JobsBoardUser> _signInManager;

        public IndexModel(
            UserManager<JobsBoardUser> userManager,
            SignInManager<JobsBoardUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 
        [Display(Name = "اسم المستخدم")]

        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "رقم الجوال")]
            public string PhoneNumber { get; set; }

            [DisplayName("تغير نوع الحساب")]
            public string UserType { get; set; }
        }

        private async Task LoadAsync(JobsBoardUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var UserType = await _userManager.GetUserAsync(User);


            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                UserType = UserType.UserType
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // تحديث رقم الهاتف إذا تغيّر
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "حدث خطأ غير متوقع عند محاولة تحديث رقم الهاتف.";
                    return RedirectToPage();
                }
            }

            // تحديث UserType والدور
            if (Input.UserType != user.UserType)
            {
                // تحديث UserType
                user.UserType = Input.UserType;
                var updateUserResult = await _userManager.UpdateAsync(user);
                if (!updateUserResult.Succeeded)
                {
                    StatusMessage = "حدث خطأ غير متوقع عند محاولة تحديث نوع المستخدم.";
                    return RedirectToPage();
                }

                // تحديد الدور الجديد بناءً على UserType
                var newRole = Input.UserType; // إما "باحث عن عمل" أو "صاحب عمل"

                // الحصول على الأدوار الحالية
                var currentRoles = await _userManager.GetRolesAsync(user);

                // إزالة الأدوار الحالية
                var removeRoleResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRoleResult.Succeeded)
                {
                    StatusMessage = "حدث خطأ غير متوقع عند محاولة إزالة الأدوار القديمة.";
                    return RedirectToPage();
                }

                // إضافة الدور الجديد
                var addRoleResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addRoleResult.Succeeded)
                {
                    StatusMessage = "حدث خطأ غير متوقع عند محاولة تعيين الدور الجديد.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "تم تحديث بياناتك بنجاح.";
            return RedirectToPage();
        }

    }
}
