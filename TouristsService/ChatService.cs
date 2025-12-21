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
        var query = _unitOfWork.Context.Set<Chat>().AsQueryable()
            .AsNoTracking()
            .Where(c => c.Participants.Any(p => p.UserId == userId) && c.LastMessageId!=null);
        
        if (beforeDate.HasValue)
            query = query.Where(c=>c.LastMessage.SentAt<beforeDate.Value);
        
        
        var dtos = await query.OrderByDescending(c => c.LastMessage.SentAt)
            .Take(pageSize + 1)
            .Select(c => new ChatListDto
            {
                ChatId = c.Id,
                PartnerId = c.Participants.FirstOrDefault(p=>p.UserId!=userId).UserId,
                PartnerName = c.Participants.FirstOrDefault(p=>p.UserId!=userId).User.UserName ,
                
                PartnerPhotoUrl = c.Participants.FirstOrDefault(p=>p.UserId!=userId).User.PhotoUrl?? "No Photo",
                
                LastMessageText = c.LastMessage.AttachmentFileId!=null? "📎 Attachment" : c.LastMessage.Text,
                LastMessageTime = c.LastMessage.SentAt,
                UnreadCount = c.Messages.Count(m =>m.ChatId==c.Id && m.Id>
                    (c.Participants.FirstOrDefault(p=>p.UserId==userId).LastSeenMessageId??0 ) )
            })
            .ToListAsync();
        
        
        bool hasmore = dtos.Count() > pageSize;
        if(hasmore)
            dtos.RemoveAt(pageSize);
        
        var nextCursor = dtos.Any() ? dtos.Last().LastMessageTime : (DateTime?)null;
        
        return new PagedResult<ChatListDto>
        {
            items = dtos,
            NextCursor = nextCursor,
            hasMore = hasmore
        };
        
    }

    public async Task<PagedResult<MessageDto>> GetChatHistoryAsync(int chatId, int? cursor, Guid userId,int pageSize=15)
    {
        var chat = await _unitOfWork.Repository<Chat>().GetByIdAsync(chatId,true,c=>c.Participants);
        if (chat == null)
            throw new KeyNotFoundException($"No Chat with id = {chatId} is found");

        if (!chat.Participants.Any(p => p.UserId == userId))
            throw new UnauthorizedAccessException("You Are Not Member In This Chat");
        
        var query = _unitOfWork.Context.Set<Message>().AsQueryable().
            AsNoTracking()
            .Where(c => c.ChatId == chatId);
        if (cursor.HasValue)
            query = query.Where(m => m.Id < cursor.Value);
        
        var partnerLastSeenId = chat.Participants
            .Where(p => p.UserId != userId)
            .Select(p => p.LastSeenMessageId)
            .FirstOrDefault() ?? 0;
        
        var dtos = await query.
            OrderByDescending(m => m.Id)
            .Take(pageSize + 1)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Text = m.Text,
                SentAt = m.SentAt,
                ReplyToMessageId = m.ReplyToMessageId,
                AttachmentUrl = m.AttachmentFile != null ? m.AttachmentFile.FilePath : null,
                AttachmentType = m.AttachmentFile != null ? m.AttachmentFile.ContentType : null,
                IsRead = (m.SenderId==userId && m.Id<=partnerLastSeenId)
                // does my partner see my message or not 
            })
            .ToListAsync();
        
        bool hasmore =  dtos.Count() > pageSize;
        
        if(hasmore)
            dtos.RemoveAt(pageSize);
        
        var nextCursor = dtos.Any() ? dtos.Last().Id : (int?)null;
        
        return new PagedResult<MessageDto>
        {
            items = dtos,
            NextCursor = nextCursor,
            hasMore = hasmore
        };
    }

    public async Task MarkMessagesAsReadAsync(MarkReadDto dto, Guid userId)
    {
        var chat = await _unitOfWork.Repository<Chat>().GetByIdAsync(dto.ChatId,true,
            c=>c.Participants);
        if (chat == null)
            throw new KeyNotFoundException($"No Chat with id = {dto.ChatId} is found");

        var me = chat.Participants.FirstOrDefault(p=>p.UserId==userId);
        if (me==null)
            throw new UnauthorizedAccessException("You Are Not Member In This Chat");
        
        if(me.LastSeenMessageId>=dto.LastSeenMessageId)
            return;

        me.LastSeenMessageId = dto.LastSeenMessageId;
        _unitOfWork.Repository<ChatParticipant>().Update(me);
        await _unitOfWork.CompleteAsync();
        
        var otherParticipant = chat.Participants.FirstOrDefault(p=>p.UserId!=userId);

        if (otherParticipant != null)
        {
            var ConnectionIds = await _tracker.GetConnections(otherParticipant.UserId.ToString());
            if (ConnectionIds.Any())
            {
                await _hubContext.Clients.Clients(ConnectionIds).SendAsync(ChatHubMethods.MarkMessagesRead, dto);
            }
        }

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