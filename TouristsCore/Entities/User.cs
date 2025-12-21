using Microsoft.AspNetCore.Identity;

namespace TouristsCore.Entities;

public class User : IdentityUser<Guid> , ISoftDeletable
{
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public TouristProfile? TouristProfile { get; set; }
    public GuideProfile? GuideProfile { get; set; }
    public ICollection<RefreshToken>  RefreshTokens { get; set; } = new List<RefreshToken>();
    public void UndoDelete()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
    public string? PhotoUrl { get; set; }
}