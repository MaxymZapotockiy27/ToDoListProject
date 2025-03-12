using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using ToDoListProj.Models;
using ToDoListProj.Services;

public class ConfirmEmailModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRedisService _redisService;

    public ConfirmEmailModel(UserManager<ApplicationUser> userManager, IRedisService redisService)
    {
        _userManager = userManager;
        _redisService = redisService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        public string Code { get; set; } // Confirmation code entered by the user
    }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnPostAsync(string userId)
    {
        if (string.IsNullOrEmpty(Input.Code))
        {
            StatusMessage = "Please enter the confirmation code.";
            return Page();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            StatusMessage = "User not found.";
            return Page();
        }

        
        if (user.EmailConfirmed)
        {
            StatusMessage = "Your email has already been confirmed.";
            return RedirectToPage("/Account/Login"); 
        }

        
        var storedCode = await _redisService.GetCodeAsync(userId);
        if (storedCode == null)
        {
            StatusMessage = "The code was not found or has expired.";
            return Page();
        }

        if (storedCode != Input.Code)
        {
            StatusMessage = "Invalid confirmation code.";
            return Page();
        }

        
        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            StatusMessage = "An error occurred while confirming the email.";
            return Page();
        }

        
        await _redisService.RemoveCodeAsync(userId);

        StatusMessage = "Thank you for confirming the email!";
        return RedirectToPage("/Account/Login");  
    }

}