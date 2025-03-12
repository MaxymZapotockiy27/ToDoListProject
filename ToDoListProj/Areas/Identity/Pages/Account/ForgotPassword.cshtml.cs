// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ToDoListProj.Models;
using ToDoListProj.Services;


namespace ToDoListProj.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
      
        private readonly ILogger<RegisterModel> _logger;
        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, ILogger<RegisterModel> logger)
        {
           
            
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }
      
        [BindProperty]
        public string ReturnUrl { get; set; } = "/";

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
            [Required]
            [EmailAddress]
            public string Email { get; set; }


        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = "/")
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User with this email address not found.");
                    return Page();
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed.");
                    return Page();
                }

                _logger.LogInformation($"Password reset request initiated for user {Input.Email}.");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Create a link for password reset
                var resetLink = Url.Page("/Account/ResetPassword", null, new { token, email = user.Email }, Request.Scheme);
                await SendEmail(Input.Email);
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }


        private async Task SendEmail(string email)
        {
            var fromMail = "todolisttodolist43@gmail.com";
            var fromPassword = "xmeo nmwe kxae pawr";
            var user = await _userManager.FindByEmailAsync(Input.Email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Create a link for password reset
            var resetLink = Url.Page("/Account/ResetPassword", null, new { token, email = user.Email }, Request.Scheme);
            try
            {
                var fromMailAddress = new MailAddress(fromMail);
                var toMailAddress = new MailAddress(email);
                var message = new MailMessage
                {
                    From = fromMailAddress,
                    Subject = "Your confirmation code",
                    IsBodyHtml = true,
                    Body = $@"
              <html>
                  <head>
                      <style>
                          body {{
                              font-family: Arial, sans-serif;
                              background-color: #f4f7fa;
                              margin: 0;
                              padding: 0;
                              text-align: center;
                              color: #333333;
                          }}
                          .container {{
                              background-color: #ffffff;
                              border-radius: 12px;
                              padding: 40px;
                              margin: 40px auto;
                              max-width: 600px;
                              box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
                          }}
                          h2 {{
                              font-size: 28px;
                              margin-bottom: 25px;
                              font-weight: bold;
                              color: #333333;
                          }}
                          p {{
                              font-size: 16px;
                              line-height: 1.6;
                              color: #555555;
                          }}
                          .code {{
                              background-color: #4CAF50; /* Green color */
                              color: white;
                              padding: 14px 30px;
                              font-size: 34px;
                              font-weight: bold;
                              border-radius: 8px;
                              display: inline-block;
                              margin: 25px 0;
                              text-decoration: none;
                              transition: background-color 0.3s ease;
                          }}
                          .code:hover {{
                              background-color: #45a049; /* Darker green on hover */
                          }}
                          .footer {{
                              color: #777777;
                              font-size: 14px;
                              margin-top: 30px;
                          }}
                          .footer a {{
                              color: #4CAF50;
                              text-decoration: none;
                          }}
                          .footer a:hover {{
                              text-decoration: underline;
                          }}
                      </style>
                  </head>
                  <body>
                      <div class='container'>
                          <h2>Your confirmation code</h2>
                          <p>Use this code to confirm your email address:</p>
                          <div class='code'>
                              <a href='{resetLink}'>Reset Password</a>
                          </div>
                          <p>If you did not make this request, please ignore this email.</p>
                          <div class='footer'>
                              <p>Best regards,<br>ToDoList Team</p>
                          </div>
                      </div>
                  </body>
              </html>"
                };

                message.To.Add(toMailAddress);

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                    smtpClient.EnableSsl = true;
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
                Console.WriteLine("Confirmation code sent! Please check your email.");
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
