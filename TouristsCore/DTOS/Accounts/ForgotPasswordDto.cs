using System.ComponentModel.DataAnnotations;

namespace TouristsCore.DTOS.Accounts;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}