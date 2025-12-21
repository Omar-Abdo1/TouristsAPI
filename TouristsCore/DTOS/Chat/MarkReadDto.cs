using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TouristsCore.DTOS.Chat;

public class MarkReadDto
{
    [Required]
    public int ChatId { get; set; }

    [Required] 
    public int LastSeenMessageId { get; set; }
}