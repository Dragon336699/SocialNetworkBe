using DataAccess.DbContext;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class ConversationUserRepository : GenericRepository<ConversationUser>, IConversationUserRepository
    {
        public ConversationUserRepository(SocialNetworkDbContext context) : base(context)
        {

        }

        public async Task<Guid?> GetConversationIdBetweenUsersAsync(Guid senderId, Guid receiverId)
        {
            Guid conversationId = await _context.ConversationUser
                .GroupBy(cu => cu.ConversationId)
                .Where(g =>
                    g.Count() == 2 && g.Any(cu => cu.UserId == senderId) && g.Any(cu => cu.UserId == receiverId))
                .Select(g => g.Key)
                .FirstOrDefaultAsync();
            return conversationId ==  Guid.Empty ? null : conversationId;
        }
    }
}
