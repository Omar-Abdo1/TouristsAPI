using Microsoft.AspNetCore.SignalR;
using Stripe.Terminal;
using TouristsAPI.Hubs;
using TouristsCore;
using TouristsCore.DTOS.Chat;
using TouristsCore.Entities;
using TouristsCore.Services;

namespace TouristsService;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ConnectionTracker _tracker;

    public ChatService(IUnitOfWork  unitOfWork,IFileService fileService,ConnectionTracker  tracker, IHubContext<ChatHub> hubContext)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _hubContext = hubContext;
        _tracker = tracker;
    }

    public async Task<MessageDto> SendMessageAsync(SendMessageDto dto, Guid senderId)
    {
        if (string.IsNullOrEmpty(dto.Text) && dto.File == null)
            throw new Exception("Message Can Not Be Empty");
        var chat = await GetOrCreatePrivateChatAsync(senderId, dto.ReceiverId);

        var message = new Message
        {
            CreatedAt = default,
            ChatId = chat.Id,
            SenderId = senderId,
            Text = dto.Text,
            ReplyToMessageId = dto.ReplyToMessageId,
            SentAt = DateTime.UtcNow
        };
        
        if (dto.File != null)
        {
            var file = await _fileService.SaveFileAsync(dto.File, "Messages", senderId);
            message.AttachmentFileId = file.Id;
            message.AttachmentFile = file;
        }
        
        _unitOfWork.Repository<Message>().Add(message);

        chat.LastMessageId = message.Id;
        
        _unitOfWork.ChatRepository.Update(chat);
        
        await _unitOfWork.CompleteAsync();

        var messageDto = new MessageDto
        {
            Id = message.Id,
            SenderId = senderId,
            Text = message.Text,
            SentAt = message.SentAt,
            IsRead = false,
            ReplyToMessageId = message.ReplyToMessageId,
            AttachmentUrl = message.AttachmentFile?.FilePath,
            AttachmentType = message.AttachmentFile?.ContentType
        };

        var Connections = await _tracker.GetConnections(dto.ReceiverId.ToString());
        
        if (Connections.Any()) // now handle Real-Time
        {
            await _hubContext.Clients.Clients(Connections).SendAsync(ChatHubMethods.ReceiveMessage, messageDto);
        }

        return messageDto;
    }

    private async Task<Chat> GetOrCreatePrivateChatAsync(Guid senderId, Guid receiverId)
    {
        var chat = await _unitOfWork.ChatRepository.GetPrivateChatAsync(senderId, receiverId);
        
        if (chat != null) return chat;

        var newChat = new Chat()
        { 
          Participants = new List<ChatParticipant>()
          {
            new ChatParticipant(){UserId = senderId},
            new ChatParticipant(){UserId = receiverId}
          }
        };
        _unitOfWork.ChatRepository.Add(newChat);
        await _unitOfWork.CompleteAsync();
        return newChat;
    }

   
}