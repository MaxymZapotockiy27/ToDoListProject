using ToDoListProj.Data;

namespace ToDoListProj.Services
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore; // Додайте це, щоб використовувати ToListAsync
    using ToDoListProj.Models;

    public class TokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public TokenService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task InvalidateAllUserSessionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Оновлюємо SecurityStamp для інвалідовування всіх сесій
                await _userManager.UpdateSecurityStampAsync(user);

                // Виходимо з системи для поточного користувача
                await _signInManager.SignOutAsync();
            }
        }
    }


}
