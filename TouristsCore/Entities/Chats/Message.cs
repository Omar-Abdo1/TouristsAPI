namespace TouristsCore.Entities;

public class Message : BaseEntity
{
    public int ChatId { get; set; }
    public Chat Chat { get; set; }
    public Guid SenderId { get; set; }
    public User Sender { get; set; }
    public string? Text { get; set; } // nullable if attachment only
    public int? AttachmentFileId { get; set; }
    public FileRecord AttachmentFile { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsDeletedGlobally { get; set; } = false; // if admin/sender deletes globally
    
    public int? ReplyToMessageId { get; set; }
    public Message? ReplyToMessage { get; set; }
    
    ICollection<Message>? ReplyToMessages { get; set; }
    
    // Navigation to the "Stickers"
    public ICollection<MessageVisibility> HiddenForUsers { get; set; }
}