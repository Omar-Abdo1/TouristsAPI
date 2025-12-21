namespace TouristsCore.Entities;

public class Chat : BaseEntity
{
    public int? LastMessageId { get; set; }
    
    public Message? LastMessage { get; set; } // to show it fast if you are out of the chat
    public ICollection<ChatParticipant> Participants { get; set; } = new  List<ChatParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}