namespace TouristsCore.Entities;

public class Chat : BaseEntity
{
    // Navigation
    public ICollection<ChatParticipant> Participants { get; set; }
    public ICollection<Message> Messages { get; set; }
}