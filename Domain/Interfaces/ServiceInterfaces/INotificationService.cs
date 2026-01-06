using Domain.Contracts.Responses.Notification;
using Domain.Entities;
using Domain.Enum.Notification.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface INotificationService
    {
        Task ProcessAndSendNotiForReactPost(NotificationType type, NotificationData? data, string navigateUrl, string mergeKey, Guid receiverId);
        Task ProcessAndSendNotiForCommentPost(NotificationType type, NotificationData data, string navigateUrl, string mergeKey, Guid receiverId);
        Task ProcessAndSendNotiForFriendRequest(NotificationType type, NotificationData data, string navigateUrl, Guid receiverId);
        Task ProcessAndSendNotiForGroupInvite(NotificationType type, NotificationData data, string navigateUrl, Guid receiverId, Guid groupId);
        Task ProcessAndSendNotiForGroupJoinRequest(NotificationType type, NotificationData data, string navigateUrl, List<Guid> adminIds, Guid groupId);
        Task ProcessAndSendNotiForGroupJoinRequestAccepted(NotificationType type, NotificationData data, string navigateUrl, Guid receiverId, Guid groupId);
        Task<List<NotificationDto>?> GetNotis(Guid userId, int skip, int take);
        Task<int> GetUnreadNotifications(Guid userId);
        Task MarkNotificationAsRead(Guid notificationId);
        Task MarkAllNotificationsAsRead();
    }
}
