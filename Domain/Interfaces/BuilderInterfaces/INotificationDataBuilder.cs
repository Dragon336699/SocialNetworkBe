using Domain.Contracts.Responses.Notification;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.BuilderInterfaces
{
    public interface INotificationDataBuilder
    {
        Task<NotificationData?> BuilderDataForReactPost(Post post, User actor, Group? group);
        NotificationData BuilderDataForComment(Post post, Comment comment, User actor);
        NotificationData BuilderDataForFriendRequest(User actor);
        NotificationData BuilderDataForAcceptFriendRequest(User actor);
    }
}
