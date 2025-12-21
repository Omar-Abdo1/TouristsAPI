using TouristsCore.Entities;

namespace TouristsCore.Repositories;

public interface IChatRepository : IGenericRepository<Chat>
{
    Task<Chat?>GetPrivateChatAsync(Guid senderId, Guid receiverId);
    // for my chat screen loaded with last message
    Task<IEnumerable<Chat>> GetUserChatsAsync(Guid senderId);
}