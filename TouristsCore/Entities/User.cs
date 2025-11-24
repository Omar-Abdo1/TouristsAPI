using Microsoft.AspNetCore.Identity;

namespace TouristsCore.Entities;

public class User : IdentityUser<Guid>
{
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public TouristProfile? TouristProfile { get; set; }
    public GuideProfile? GuideProfile { get; set; }
    public ICollection<RefreshToken>  RefreshTokens { get; set; } = new List<RefreshToken>();
}