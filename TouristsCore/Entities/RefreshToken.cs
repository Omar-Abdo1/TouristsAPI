using Microsoft.EntityFrameworkCore;

namespace TouristsCore.Entities;

[Owned]
public class RefreshToken
{
    public string Token { get; set; }
    public DateTime ExpiresOn { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedOn { get; set; }
    
    public bool IsActive => RevokedOn is null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
}