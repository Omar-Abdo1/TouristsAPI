namespace TouristsCore.DTOS.Accounts;

public class UserProfileDto
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    
    public List<string> LinkedProviders { get; set; } // e.g. ["Google", "Facebook"]
    public bool HasPassword { get; set; }
    
    public string? AvatarUrl { get; set; }
}