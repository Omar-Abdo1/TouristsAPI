using System.ComponentModel.DataAnnotations;

namespace TouristsCore.DTOS.Accounts;

public class SocialLoginDto
{
    [Required]
    public string Token { get; set; } // The ID Token from Google
    [Required]
    // Frontend sends "Tourist" or "Guide"
    [RegularExpression("Tourist|Guide", ErrorMessage = "Role must be either Tourist or Guide.")]
    public string Role { get; set; } = "Tourist"; // Default role if they are new
}