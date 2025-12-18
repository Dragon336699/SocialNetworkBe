using Domain.Contracts.Responses.Notification;
using Domain.Entities;
using Domain.Enum.Notification.Types;
using Domain.Interfaces.BuilderInterfaces;
using Domain.Interfaces.ServiceInterfaces;

namespace SocialNetworkBe.Services.NotificationServices.NotificationDataBuilder
{
    public class NotificationDataBuilder : INotificationDataBuilder
    {
        private readonly IPostReactionUserService _postReactionUserService;
        public NotificationDataBuilder(IPostReactionUserService postReactionUserService)
        {
            _postReactionUserService = postReactionUserService;
        }
        public async Task<NotificationData?> BuilderDataForReactPost(Post post, User actor, Group? group)
        {
            List<NotificationObject> subjects = new List<NotificationObject>();
            IEnumerable<PostReactionUser> postReactionUsers = await _postReactionUserService.GetPostReactionUsersByPostId(post.Id);
            if (postReactionUsers.Any(pru => pru.UserId == actor.Id && pru.PostId == post.Id))
            {
                return null;
            }

            NotificationObject subject = new NotificationObject
            {
                Id = actor.Id,
                Name = actor.LastName + " " + actor.FirstName,
                Type = NotificationObjectType.Actor,
            };

            subjects.Add(subject);

            NotificationObject diObject = new NotificationObject
            {
                Id = post.Id,
                Type = NotificationObjectType.Post,
            };

            NotificationData notidData = new NotificationData
            {
                Subjects = subjects,
                SubjectCount = subjects.Count,
                Verb = Verb.Liked,
                DiObject = diObject,
                InObject = group != null ? new NotificationObject
                {
                    Id = group.Id,
                    Name = group.Name,
                    Type = NotificationObjectType.Group
                } : null
            };

            return notidData;
        }

        public NotificationData BuilderDataForComment(Post post, Comment comment, User actor)
        {
            List<NotificationObject> subjects = new List<NotificationObject>();

            NotificationObject subject = new NotificationObject
            {
                Id = actor.Id,
                Name = actor.LastName + " " + actor.FirstName,
                Type = NotificationObjectType.Actor,
            };

            subjects.Add(subject);

            NotificationObject diObject = new NotificationObject
            {
                Id = comment.Id,
                Type = NotificationObjectType.Comment
            };

            NotificationObject prObject = new NotificationObject
            {
                Id = post.Id,
                Type = NotificationObjectType.Post
            };

            NotificationData notidData = new NotificationData
            {
                Subjects = subjects,
                SubjectCount = subjects.Count,
                Verb = Verb.Commented,
                DiObject = diObject,
                PrObject = prObject,
                Preposition = Preposition.On
            };

            return notidData;
        }

        public NotificationData BuilderDataForFriendRequest(User actor)
        {
            List<NotificationObject> subjects = new List<NotificationObject>();

            NotificationObject subject = new NotificationObject
            {
                Id = actor.Id,
                Name = actor.LastName + " " + actor.FirstName,
                Type = NotificationObjectType.Actor,
            };

            subjects.Add(subject);

            NotificationObject diObject = new NotificationObject
            {
                Type = NotificationObjectType.FriendRequest,
                Name = "a friend request"
            };

            NotificationData notidData = new NotificationData
            {
                Subjects = subjects,
                SubjectCount = subjects.Count,
                Verb = Verb.Sent,
                DiObject = diObject,
            };

            return notidData;
        }

        public NotificationData BuilderDataForAcceptFriendRequest(User actor)
        {
            List<NotificationObject> subjects = new List<NotificationObject>();

            NotificationObject subject = new NotificationObject
            {
                Id = actor.Id,
                Name = actor.LastName + " " + actor.FirstName,
                Type = NotificationObjectType.Actor,
            };

            subjects.Add(subject);

            NotificationObject diObject = new NotificationObject
            {
                Type = NotificationObjectType.AccepFriendRequest,
                Name = "friend request"
            };

            NotificationData notidData = new NotificationData
            {
                Subjects = subjects,
                SubjectCount = subjects.Count,
                Verb = Verb.Accepted,
                DiObject = diObject,
            };

            return notidData;
        }
    }
}
