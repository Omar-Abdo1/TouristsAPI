using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TouristsCore.DTOS.Chat;

public class SendMessageDto
{
    [Required]
    public Guid ReceiverId { get; set; }
    public string? Text { get; set; }
    public IFormFile?  File { get; set; }
    public int? ReplyToMessageId { get; set; }
}