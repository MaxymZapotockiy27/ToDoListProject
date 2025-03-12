// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ToDoListProj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using System.Net.Mail;
using System.Net;
using ToDoListProj.Services;

namespace ToDoListProj.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CodeGenerator _codeGenerator;
        private readonly IRedisService _redisService;



        public LoginModel(SignInManager<ApplicationUser> signInManager,IRedisService redisService,CodeGenerator codeGenerator, UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger)
        {
            _redisService = redisService;
            _codeGenerator = codeGenerator;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

    
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

             var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user != null && !user.EmailConfirmed)
            {
                 await SendEmail(user.Id, Input.Email);
                StatusMessage = "Email not yet confirmed. A new verification code has been sent to your email.";

                 return RedirectToPage("ConfirmEmail", new { userId = user.Id, email = user.Email });
            }

             var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return RedirectToAction("Index", "ToDoList");   
            }
            else if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        private async Task SendEmail(string userId, string email)
        {
            string code = _codeGenerator.GenerateCode();  // Generate the confirmation code
            await _redisService.SetCodeAsync(userId, code);  // Store the code in Redis

            var fromMail = "todolisttodolist43@gmail.com";
            var fromPassword = "xmeo nmwe kxae pawr";

            try
            {
                var fromMailAddress = new MailAddress(fromMail);
                var toMailAddress = new MailAddress(email);
                var message = new MailMessage
                {
                    From = fromMailAddress,
                    Subject = "Your confirmation code ",
                    IsBodyHtml = true,
                    Body = $"<html><body><h2>confirmation code : <b>{code}</b></h2></body></html>"
                };
                message.To.Add(toMailAddress);

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                    smtpClient.EnableSsl = true;
                    Console.WriteLine($"[DEBUG] Sending email to {email} with code: {code}");
                    Console.WriteLine($"[DEBUG] Sender: {fromMail}");
                    Console.WriteLine($"[DEBUG] Using password: {(string.IsNullOrEmpty(fromPassword) ? "NOT SET" : "SET")}");

                    try
                    {
                        await smtpClient.SendMailAsync(message);
                        Console.WriteLine("✅ Confirmation code sent successfully!");
                    }
                    catch (SmtpException smtpEx)
                    {
                        Console.WriteLine($"❌ SMTP Error: {smtpEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ General error: {ex.Message}");
                    }

                }
                Console.WriteLine("The confirmation code has been sent! Please check your email.");
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine("Error sending email: " + smtpEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }



    }
}