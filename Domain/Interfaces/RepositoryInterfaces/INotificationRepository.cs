using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<List<Notification>> GetNotis(Guid userId, int skip, int take);
        Task<int> GetUnreadNotifications(Guid userId);
        Task MarkAllNotificationsAsRead();
    }
}
