using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TouristsCore.DTOS.Chat;

public class SendMessageDto
{
    [Required]
    public Guid ReceiverId { get; set; }
    public string? Text { get; set; }
    public int?  AttachmentId { get; set; }
    public int? ReplyToMessageId { get; set; }
}