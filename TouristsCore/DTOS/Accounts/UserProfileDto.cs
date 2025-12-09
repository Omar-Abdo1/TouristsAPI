namespace TouristsCore.DTOS.Accounts;

public class UserProfileDto
{
    //Shared
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    
    public List<string> LinkedProviders { get; set; } // e.g. ["Google", "Facebook"]
    public bool HasPassword { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    public string?FullName { get; set; }
    public string? Phone { get; set; }
    
    //Tourist
    public string? Country { get; set; }
    
    //Guide
    public string Bio { get; set; }
    public int? ExperienceYears { get; set; }
    public decimal? RatePerHour { get; set; }
    
}