using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Stripe.Terminal;
using TouristsAPI.Hubs;
using TouristsCore;
using TouristsCore.DTOS.Chat;
using TouristsCore.Entities;
using TouristsCore.Services;
using TouristsService.Pagination;

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

        var me = chat.Participants.First(p => p.UserId == senderId);
        
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

    public async Task<PagedResult<ChatListDto>> GetUserChatsAsync(Guid userId,DateTime? beforeDate,int pageSize=15)
    {
        throw new NotImplementedException();
        // var query = _unitOfWork.Context.Set<Chat>().AsQueryable()
        //     .AsNoTracking()
        //     .Include(c => c.LastMessage)
        //     .Include(c => c.Participants).ThenInclude(p => p.User)
        //     .Where(c => c.Participants.Any(p => p.UserId == userId) && c.LastMessageId!=null);
        //
        // if (beforeDate.HasValue)
        //     query = query.Where(c=>c.LastMessage.CreatedAt<beforeDate.Value);
        //
        // var dtos = await query.OrderByDescending(c => c.LastMessage.CreatedAt)
        //     .Take(pageSize + 1)
        //     .Select(c => new ChatListDto
        //     {
        //         ChatId = c.Id,
        //         PartnerId = c.Participants.FirstOrDefault(p=>p.UserId!=userId).UserId,
        //         PartnerName = c.Participants.FirstOrDefault(p=>p.UserId!=userId).User.UserName ,
        //         
        //         PartnerPhotoUrl = c.Participants.FirstOrDefault(p => p.UserId != userId).User.TouristProfile != null 
        //             ? c.Participants.FirstOrDefault(p => p.UserId != userId).User.TouristProfile.AvatarFile.FilePath 
        //             : c.Participants.FirstOrDefault(p => p.UserId != userId).User.GuideProfile.ava,
        //         
        //         LastMessageText = c.LastMessage.AttachmentFileId != null ? "📎 Attachment" : c.LastMessage.Text,
        //         LastMessageTime = c.LastMessage.SentAt,
        //         UnreadCount = c.Messages.Count(m =>m.ChatId==c.id &&  )
        //     })
        //     .ToListAsync();
        //
        //
        // bool hasmore = dtos.Count() > pageSize;
        // if(hasmore)
        //     dtos.RemoveAt(pageSize);
        //
        // var nextCursor = dtos.Any() ? dtos.Last().LastMessageTime : (DateTime?)null;
        //
        // return new PagedResult<ChatListDto>
        // {
        //     items = dtos,
        //     NextCursor = nextCursor,
        //     hasMore = hasmore
        // };
        
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