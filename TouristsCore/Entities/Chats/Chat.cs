namespace TouristsCore.Entities;

public class Chat
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Navigation
    public ICollection<ChatParticipant> Participants { get; set; }
    public ICollection<Message> Messages { get; set; }
}