namespace TouristsCore.Entities;

public class MessageVisibility
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; } // If this row exists, it implies deleted/hidden
    public Message Message { get; set; }
    public User User { get; set; }
    public DateTime DeletedAt { get; set; }=DateTime.UtcNow;
    // composite PK: MessageId + UserId
}