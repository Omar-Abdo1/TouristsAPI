using TouristsCore.DTOS.Chat;
using TouristsService.Pagination;

namespace TouristsCore.Services;

public interface IChatService
{
    Task<MessageDto> SendMessageAsync(SendMessageDto dto, Guid senderId);
    public Task<PagedResult<ChatListDto>> GetUserChatsAsync(Guid userId, DateTime? beforeDate, int pageSize = 15);
}