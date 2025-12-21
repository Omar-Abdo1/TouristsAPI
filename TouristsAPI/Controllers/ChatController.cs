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

    [HttpGet("my-chats")]
    public async Task<IActionResult> GetMyChats([FromQuery]DateTime? LastMessageAt,[FromQuery]int pageSize=15) // the outSide UI before going to specific chat
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _chatService.GetUserChatsAsync(userId,LastMessageAt,pageSize);
        return Ok(result);
    }

    // [HttpGet("{chatId:int}/messages")]
    // public async Task<IActionResult> GetHistory(int chatId,[FromQuery]int?cursor)
    // {
    //     var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    //     var result = await _chatService.GetChatHistoryAsync(chatId,cursor,userId);
    //     return Ok(result);
    // }
    //
    // [HttpPost("read")]
    // public async Task<IActionResult> MarkRead([FromBody] MarkReadDto dto) // Updates DB and notifies the sender via SignalR
    // {
    //     var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    //     try
    //     {
    //         await _chatService.MarkMessagesAsReadAsync(dto.ChatId, userId); 
    //          return Ok();
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new ApiErrorResponse(400, ex.Message));
    //     }
    // }
    //
    // [HttpDelete("message/{id:int}")]    
    // public async Task<IActionResult> DeleteMessage(int id, [FromQuery] bool forEveryone = false) // Updates DB and notifies the sender via SignalR
    // {
    //     var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    //     try
    //     {
    //      await _chatService.DeleteMessageAsync(id, userId, forEveryone);
    //      
    //       return Ok(new { Message = "Deleted successfully" });
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new ApiErrorResponse(400, ex.Message));
    //     }
    // }
    
}