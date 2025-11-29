namespace TouristsCore.Entities;

public class ChatParticipant : BaseEntity
{
    public int ChatId { get; set; }
    public Chat Chat { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public int? LastSeenMessageId { get; set; } // nullable
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    // PK composite: ChatId + UserId
}