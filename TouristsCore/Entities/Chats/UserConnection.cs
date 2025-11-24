namespace TouristsCore.Entities;

public class UserConnection // for SignalR
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string ConnectionId { get; set; }
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DisconnectedAt { get; set; }
}