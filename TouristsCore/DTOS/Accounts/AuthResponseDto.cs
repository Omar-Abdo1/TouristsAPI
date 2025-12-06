namespace TouristsCore.DTOS.Accounts;

public class AuthResponseDto
{
    public bool IsAuthenticated { get; set; } = false;
    public string Message { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiresOn { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
}