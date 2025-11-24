namespace TouristsCore.Entities;

public class UserGroup // for SignalR
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string GroupName { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // composite PK: UserId + GroupName
}