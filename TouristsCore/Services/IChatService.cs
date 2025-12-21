using TouristsCore.DTOS.Chat;

namespace TouristsCore.Services;

public interface IChatService
{
    Task<MessageDto>SendMessageAsync(SendMessageDto dto,Guid senderId);
}