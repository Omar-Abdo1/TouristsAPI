using Microsoft.AspNetCore.Identity;

namespace TouristsCore.Entities;

public class User : IdentityUser
{
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public TouristProfile? TouristProfile { get; set; }
    public GuideProfile? GuideProfile { get; set; }
}