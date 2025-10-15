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
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(SocialNetworkDbContext context) : base(context)
        {
            
        }

        public async Task<List<Message>?> GetMessages(Guid senderId, Guid receiverId)
        {
            var messages = await _context.Set<Message>().Where(x => (x.SenderId == senderId && x.ReceiverId == receiverId) || (x.SenderId == receiverId && x.ReceiverId == senderId)).OrderBy(m => m.CreatedAt).ToListAsync();
            if (messages == null) return null;
            return messages;
        }
    }
}
