namespace TouristsCore.Entities;

public class ChatParticipant
{
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid? LastSeenMessageId { get; set; } // nullable
    public bool IsDeleted { get; set; } = false; // soft-delete for this user only
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    // PK composite: ChatId + UserId
}