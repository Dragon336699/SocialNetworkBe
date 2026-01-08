using Domain.Contracts.Responses.Notification;
using Domain.Entities;
using Domain.Enum.Notification.Types;
using Domain.Interfaces.BuilderInterfaces;
using Domain.Interfaces.ServiceInterfaces;

namespace SocialNetworkBe.Services.NotificationServices.NotificationBuilder
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
                SnapshotText = actor.LastName + " " + actor.FirstName
            };

            subjects.Add(subject);

            NotificationObject diObject = new NotificationObject
            {
                Id = post.Id,
                Type = NotificationObjectType.Post,
                SnapshotText = post.Content.Length > 30 ? post.Content.Substring(0, 30) + "..." : post.Content
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
                    Type = NotificationObjectType.Group,
                    SnapshotText= group.Name,
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
                SnapshotText = actor.LastName + " " + actor.FirstName
            };

            subjects.Add(subject);

            NotificationObject diObject = new NotificationObject
            {
                Id = comment.Id,
                Type = NotificationObjectType.Comment,
                SnapshotText = comment.Content.Length > 30 ? comment.Content.Substring(0, 20) + "..." : comment.Content
            };

            NotificationObject prObject = new NotificationObject
            {
                Id = post.Id,
                Type = NotificationObjectType.Post,
                SnapshotText = post.Content.Length > 30 ? post.Content.Substring(0, 30) + "..." : post.Content
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
                SnapshotText = actor.LastName + " " + actor.FirstName
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
                SnapshotText = actor.LastName + " " + actor.FirstName
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

        public async Task<NotificationData?> BuilderDataForGroupInvite(Group group, User inviter, User invitee)
        {
            List<NotificationObject> subjects = new List<NotificationObject>();

            NotificationObject subject = new NotificationObject
            {
                Id = inviter.Id,
                Name = inviter.LastName + " " + inviter.FirstName,
                Type = NotificationObjectType.Actor,
                SnapshotText = inviter.LastName + " " + inviter.FirstName
            };

            subjects.Add(subject);

            NotificationObject diObject = new NotificationObject
            {
                Type = NotificationObjectType.GroupInvite,
                Name = "to join"
            };

            NotificationObject inObject = new NotificationObject
            {
                Id = group.Id,
                Name = group.Name,
                Type = NotificationObjectType.Group,
                SnapshotText = group.Name
            };

            NotificationData notidData = new NotificationData
            {
                Subjects = subjects,
                SubjectCount = subjects.Count,
                Verb = Verb.Invited,
                DiObject = diObject,
                InObject = inObject
            };

            return notidData;
        }

        public NotificationData BuilderDataForGroupJoinRequest(Group group, User requester)
        {
            List<NotificationObject> subjects = new List<NotificationObject>();

            NotificationObject subject = new NotificationObject
            {
                Id = requester.Id,
                Name = requester.LastName + " " + requester.FirstName,
                Type = NotificationObjectType.Actor,
            };

            subjects.Add(subject);

            NotificationObject diObject = new NotificationObject
            {
                Type = NotificationObjectType.GroupJoinRequest,
                Name = "to join"
            };

            NotificationObject inObject = new NotificationObject
            {
                Id = group.Id,
                Name = group.Name,
                Type = NotificationObjectType.Group
            };

            NotificationData notidData = new NotificationData
            {
                Subjects = subjects,
                SubjectCount = subjects.Count,
                Verb = Verb.Requested,
                DiObject = diObject,
                InObject = inObject
            };

            return notidData;
        }

        public NotificationData BuilderDataForGroupJoinRequestAccepted(Group group)
        {
            List<NotificationObject> subjects = new List<NotificationObject>();

            NotificationObject subject = new NotificationObject
            {
                Id = group.Id,
                Name = group.Name,
                Type = NotificationObjectType.Group,
            };

            subjects.Add(subject);

            NotificationObject diObject = new NotificationObject
            {
                Type = NotificationObjectType.GroupJoinRequestAccepted,
                Name = "your request to join"
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
