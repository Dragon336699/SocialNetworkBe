using Domain.Contracts.Responses.Notification;
using Domain.Entities;
using Domain.Interfaces.BuilderInterfaces;

namespace SocialNetworkBe.Services.NotificationServices.NotificationDataBuilder
{
    public class NotificationDataBuilder : INotificationDataBuilder
    {
        public NotificationData BuilderDataForReactPost(Post post, User actor, Group? group)
        {
            List<NotificationObject> subjects = new List<NotificationObject>();

            NotificationObject subject = new NotificationObject
            {
                Id = actor.Id,
                Name = actor.LastName + " " + actor.FirstName,
                Type = "Actor",
                ImageUrl = actor.AvatarUrl
            };

            subjects.Add(subject);

            NotificationObject diObject = new NotificationObject
            {
                Id = post.Id,
                Name = post.Content.Length > 20 ? post.Content.Substring(0, 20) + "..." : post.Content,
                Type = "Post",
            };

            NotificationData notidData = new NotificationData
            {
                Subjects = subjects,
                SubjectCount = subjects.Count,
                Content = "",
                DiObject = diObject,
                InObject = group != null ? new NotificationObject
                {
                    Id = group.Id,
                    Name = group.Name,
                    Type = "Group"
                } : null
            };

            return notidData;
        }
    }
}
