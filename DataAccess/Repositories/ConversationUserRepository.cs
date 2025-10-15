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

        public async Task<Guid?> GetConversationIdBetweenUserAsync(Guid senderId, Guid receiverId)
        {
            Guid conversationId = await  _context.ConversationUser.Where(x => x.UserId == senderId)
                .Join(_context.ConversationUser
                    .Where(y => y.UserId == receiverId), x => x.ConversationId, y => y.ConversationId, (x, y) => x.ConversationId)
                .FirstOrDefaultAsync();
            return conversationId ==  Guid.Empty ? null : conversationId;
        }
    }
}
