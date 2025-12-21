namespace TouristsCore.DTOS.Chat;

public class MessageDto
{
    public int Id { get; set; }
    public Guid SenderId { get; set; }
    public string? Text { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    public int? ReplyToMessageId { get; set; }
    
    // Add these for attachments
    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; }
}