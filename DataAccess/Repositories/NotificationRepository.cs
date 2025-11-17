using DataAccess.DbContext;
using Domain.Entities;
using Domain.Enum.Message.Types;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(SocialNetworkDbContext context) : base(context)
        {
            
        }

        public async Task<List<Notification>> GetNotis (Guid userId, int skip = 0, int take = 10)
        {
            List<Notification> notis = await _context.Notification.Where(n => n.ReceiverId == userId)
                                                                    .OrderByDescending(n => n.UpdatedAt)
                                                                    .Skip(skip)
                                                                    .Take(take)
                                                                    .ToListAsync();
            return notis;
        }

        public async Task<int> GetUnreadNotifications(Guid userId)
        {
            int count = 0;
            count = await _context.Notification.Where(n => n.Unread == true && n.ReceiverId == userId).CountAsync();
            return count;
        }

        public async Task MarkAllNotificationsAsRead()
        {
            await _context.Notification.Where(n => n.Unread).ExecuteUpdateAsync(n => n.SetProperty(x => x.Unread, false));
        }
    }
}
