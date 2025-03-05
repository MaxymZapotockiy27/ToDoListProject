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
            StatusMessage = "Будь ласка, введіть код підтвердження.";
            return Page();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            StatusMessage = "Користувача не знайдено.";
            return Page();
        }

        // Отримуємо збережений код з Redis
        var storedCode = await _redisService.GetCodeAsync(userId);
        if (storedCode == null)
        {
            StatusMessage = "Код не знайдено або він протермінований.";
            return Page();
        }

        if (storedCode != Input.Code)
        {
            StatusMessage = "Невірний код підтвердження.";
            return Page();
        }

        // ✅ Оновлюємо статус підтвердження email вручну
        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            StatusMessage = "Сталася помилка при підтвердженні email.";
            return Page();
        }

        // Видаляємо код з Redis після успішного підтвердження
        await _redisService.RemoveCodeAsync(userId);

        StatusMessage = "Дякуємо за підтвердження email!";
        return RedirectToPage("/Account/Login"); // Направляємо на вхід після підтвердження
    }

}