// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ToDoListProj.Models;
using ToDoListProj.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;


namespace ToDoListProj.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IRedisService _redisService;
        private readonly CodeGenerator _codeGenerator;

        public RegisterModel(
            CodeGenerator codeGenerator,
            IRedisService redisService,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _redisService = redisService;
            _codeGenerator = codeGenerator;
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
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

        public string AvatarUrl { get; set; }

        public string StatusMessage { get; set; }

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
           // Додано поле для FullName

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            [UniqueEmail]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }   
            public string AvatarUrl { get; set; }
        


        }



        public async Task OnGetAsync(string returnUrl = null)
        {

            ReturnUrl = returnUrl ?? Url.Content("~/");  
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }



        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

             var existingUser = await _userManager.FindByEmailAsync(Input.Email);
            if (existingUser != null)
            {
                if (!existingUser.EmailConfirmed)
                {
                     await SendEmail(existingUser.Id, Input.Email);
                    StatusMessage = "Email ще не підтверджено. Новий код підтвердження надіслано на вашу пошту.";

                     return RedirectToPage("ConfirmEmail", new { userId = existingUser.Id, email = existingUser.Email });
                }
                else
                {
                     StatusMessage = "Цей email вже в використанні.";
                    return Page();
                }
            }

             if (!ModelState.IsValid)
            {
                return Page();
            }

             var newUser = CreateUser();
            newUser.FullName = Input.FullName;
            newUser.AvatarUrl = Input.AvatarUrl;
            await _userStore.SetUserNameAsync(newUser, Input.Email, CancellationToken.None);
            await _userManager.SetEmailAsync(newUser, Input.Email);

            var result = await _userManager.CreateAsync(newUser, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var userId = await _userManager.GetUserIdAsync(newUser);
                await SendEmail(userId, Input.Email);

                 return RedirectToPage("ConfirmEmail", new { userId = userId, email = newUser.Email });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }






        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
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