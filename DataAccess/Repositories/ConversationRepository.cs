using DataAccess.DbContext;
using Domain.Entities;
using Domain.Enum.Conversation.Types;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
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
                .FirstOrDefaultAsync(c => c.Type == ConversationType.Personal &&
                                          c.ConversationUsers.Count == 2 &&
                                          c.ConversationUsers.Any(cu => cu.UserId == user1Id) &&
                                          c.ConversationUsers.Any(cu => cu.UserId == user2Id));
        }

        public async Task<Message> AddOrUpdateMessageAsync(Message message)
        {
            var existingMessage = await _context.Set<Message>().FindAsync(message.Id);
            if (existingMessage != null)
            {
                _context.Entry(existingMessage).CurrentValues.SetValues(message);
            }
            else
            {
                await _context.Set<Message>().AddAsync(message);
            }
            await _context.SaveChangesAsync(); // Lưu ngay để đảm bảo message được tạo
            return message;
        }
    }
}