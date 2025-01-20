using System.ComponentModel.DataAnnotations;

namespace Sun.Models.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email")]
    public string Email { get; set; }
}