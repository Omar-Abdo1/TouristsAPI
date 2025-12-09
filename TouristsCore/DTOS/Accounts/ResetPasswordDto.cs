using System.ComponentModel.DataAnnotations;

namespace TouristsCore.DTOS.Accounts;

public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public string NewPassword { get; set; }
}