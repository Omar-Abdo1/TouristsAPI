using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TouristsCore.Services;

namespace TouristsAPI.Hubs;
[Authorize]
public class ChatHub : Hub
{
    private readonly ConnectionTracker _tracker;
    private readonly IChatService _chatService;
    
    public ChatHub(ConnectionTracker tracker, IChatService chatService)
    {
        _tracker = tracker;
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _tracker.UserConnected(userId, Context.ConnectionId);
        await Clients.All.SendAsync(ChatHubMethods.UserIsOnline, userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _tracker.UserDisconnected(userId, Context.ConnectionId);
        await Clients.All.SendAsync(ChatHubMethods.UserIsOffline, userId);
        await base.OnDisconnectedAsync(exception);
    }
}