using DataAccess.DbContext;
using Domain.Contracts.Responses.Message;
using Domain.Entities;
using Domain.Enum.Message.Types;
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

        public async Task<List<Message>?> GetMessages(Guid conversationId, int skip, int take)
        {
            var messages = await _context
                .Set<Message>()
                .Where(x => x.ConversationId == conversationId)
                .Include(x => x.Sender)
                .Include(x => x.MessageAttachments)
                .Include(x => x.MessageReactionUsers)
                .Include(x => x.RepliedMessage)
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
            if (messages == null) return null;
            return messages;
        }

        public async Task<Message?> UpdateAllMessagesStatus(Guid messageId, MessageStatus messageStatus)
        {
            Message? message = await _context
                .Set<Message>()
                .Where(x => x.Id == messageId)
                .FirstAsync();

            if (message == null) return null;

            var allMessages = await _context
                .Set<Message>()
                .Where(m => m.ConversationId == message.ConversationId && m.SenderId == message.SenderId && m.Status != messageStatus)
                .ToListAsync();
            foreach (var m in allMessages) m.Status = messageStatus;
            await _context.SaveChangesAsync();
            return message;
        }
    }
}
