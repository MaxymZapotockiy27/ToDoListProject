using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ToDoListProj.Models;

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var userManager = (UserManager<ApplicationUser>)validationContext
            .GetService(typeof(UserManager<ApplicationUser>));

        var email = value as string;

        if (userManager.Users.Any(u => u.Email == email))
        {
            return new ValidationResult("This email is already in use.");
        }

        return ValidationResult.Success;
    }
}
