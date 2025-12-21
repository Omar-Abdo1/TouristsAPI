using TouristsCore.DTOS.Chat;
using TouristsService.Pagination;

namespace TouristsCore.Services;

public interface IChatService
{
    Task<MessageDto> SendMessageAsync(SendMessageDto dto, Guid senderId);
    Task<PagedResult<ChatListDto>> GetUserChatsAsync(Guid userId, DateTime? beforeDate, int pageSize = 15);
    Task<PagedResult<MessageDto>> GetChatHistoryAsync(int chatId, int? cursor, Guid userId,int pageSize=15);
    Task MarkMessagesAsReadAsync(MarkReadDto dto, Guid userId);
    Task DeleteMessageAsync(int id, Guid userId, bool forEveryone);
}