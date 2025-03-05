// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using ToDoListProj.Models;

namespace ToDoListProj.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
       [BindProperty]
        public string ResetToken { get; set; }

        // Змінив ім'я властивості з Email на UserEmail
        [BindProperty]
        public string UserEmail { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            

         

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
          

        }

        //public IActionResult OnGet(string token, string email, string returnUrl = null)
        //{
        //    if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        //    {
        //        return RedirectToPage("/Index");
        //    }
        //    ReturnUrl = returnUrl ?? Url.Content("~/");

        //    ResetToken = token;
        //    UserEmail = email; // Ensure this email is not null

        //    return Page();
        //}
        public IActionResult OnGet(string token, string email, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Index"); // Перенаправлення на головну сторінку, якщо немає токена або email
            }

            // Перевіряємо, чи дійсний токен (можна реалізувати перевірку за допомогою методу із UserManager)
            var user = _userManager.FindByEmailAsync(email).Result;
            if (user == null)
            {
                return RedirectToPage("/Index"); // Якщо користувача не знайдено, перенаправляємо на головну
            }

            var tokenValid = _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token).Result;
            if (!tokenValid)
            {
                return RedirectToPage("/Error"); // Перенаправляємо на сторінку помилки, якщо токен недійсний
            }

            ReturnUrl = returnUrl ?? Url.Content("~/"); // Якщо returnUrl не передано, використовуємо головну сторінку
            ResetToken = token;
            UserEmail = email;

            return Page();
        }




        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Отримуємо користувача за email
            var user = await _userManager.FindByEmailAsync(UserEmail);
            if (user == null)
            {
                return RedirectToPage("/Index");
            }

            // Скидаємо пароль
            var result = await _userManager.ResetPasswordAsync(user, ResetToken, Input.Password);

            if (result.Succeeded)
            {
                // Оновлюємо SecurityStamp, щоб інвалідовувати всі старі сесії
                await _userManager.UpdateSecurityStampAsync(user);

              
                await _signInManager.SignOutAsync();

                // Перенаправляємо на сторінку входу
                return RedirectToPage("./Login");
            }

            // Додаємо помилки в модель, якщо скидання пароля не вдалося
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

    }
}
