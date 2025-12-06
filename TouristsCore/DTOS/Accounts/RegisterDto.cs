using System.ComponentModel.DataAnnotations;

namespace TouristsCore.DTOS.Accounts;

public class RegisterDto
{
    [Required]
    public string Username { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public string? PhoneNumber { get; set; }

    [Required]
    // Frontend sends "Tourist" or "Guide"
    [RegularExpression("Tourist|Guide", ErrorMessage = "Role must be either Tourist or Guide.")]
    public string Role { get; set; }
}