using System.ComponentModel.DataAnnotations;

namespace Sun.Models.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string Token { get; set; }  // Токен для сброса пароля
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
