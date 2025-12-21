namespace TouristsCore.DTOS.Chat;

public class ChatListDto
{
    public int ChatId { get; set; }
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; }
    public string? PartnerPhotoUrl { get; set; }
    public string? LastMessageText { get; set; }
    public DateTime LastMessageTime { get; set; }
    public int UnreadCount { get; set; }
}