using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristsAPI.ErrorResponses;
using TouristsCore.DTOS.Chat;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService  chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromForm] SendMessageDto messageDto) // accepts text+file
    {
        try
        {
            var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result =await _chatService.SendMessageAsync(messageDto, senderId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
}