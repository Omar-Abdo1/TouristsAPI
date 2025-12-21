using Microsoft.EntityFrameworkCore;
using TouristsCore.Entities;
using TouristsCore.Repositories;

namespace TouristsRepository;

public class ChatRepository : GenericRepository<Chat> , IChatRepository
{
    private readonly TouristsContext _context;
    public ChatRepository(TouristsContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Chat?> GetPrivateChatAsync(Guid senderId, Guid receiverId)
    {
        return await _context.Chats.Include(c => c.Participants)
            .Where(c => c.Participants.Any(p => p.UserId == senderId) &&
                        c.Participants.Any(p => p.UserId == receiverId))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Chat>> GetUserChatsAsync(Guid senderId)
    {
        return await _context.Chats.Include(c => c.LastMessage)
            .Include(c => c.Participants)
            .Where(c => c.Participants.Any(p => p.UserId == senderId))
            .OrderByDescending(c => c.LastMessage != null 
                ? c.LastMessage.SentAt 
                : DateTime.MinValue)            
                .ToListAsync();
    }
}