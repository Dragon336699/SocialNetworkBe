using DataAccess.DbContext;
using Domain.Entities;
using Domain.Enum.MessageAttachment.Types;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class MessageAttachmentRepository : GenericRepository<MessageAttachment>, IMessageAttachmentRepository
    {
        public MessageAttachmentRepository(SocialNetworkDbContext context): base(context)
        {
            
        }

        public async Task<List<MessageAttachment>?> GetImageAttachmentsByConversationId(Guid conversationId, int skip, int take)
        {
            var attachments = await _context
                .Set<MessageAttachment>()
                .Where(x => x.Message.ConversationId == conversationId && x.FileType == FileTypes.Image)
                .Include(x => x.Message)
                .AsNoTracking()
                .OrderByDescending(x => x.Message.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return attachments;
        }
    }
}
