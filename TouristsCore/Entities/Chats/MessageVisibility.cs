namespace TouristsCore.Entities;

public class MessageVisibility : BaseEntity
{
    public int MessageId { get; set; }
    public Guid UserId { get; set; } // If this row exists, it implies deleted/hidden
    public Message Message { get; set; }
    public User User { get; set; }
    // composite PK: MessageId + UserId
}