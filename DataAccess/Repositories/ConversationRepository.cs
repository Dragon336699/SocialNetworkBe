using DataAccess.DbContext;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(SocialNetworkDbContext context) : base(context)
        {
            
        }

        public async Task<Conversation?> GetConversationBetweenUsersAsync(Guid user1Id, Guid user2Id)
        {
            // Truy vấn DB để tìm cuộc hội thoại giữa hai người dùng
            return await _context.Set<Conversation>()
                .Include(c => c.ConversationUsers)
                .FirstOrDefaultAsync(c => c.ConversationUsers.Count == 2 &&
                                          c.ConversationUsers.Any(cu => cu.UserId == user1Id) &&  
                                          c.ConversationUsers.Any(cu => cu.UserId == user2Id));  
        }
    }
}
