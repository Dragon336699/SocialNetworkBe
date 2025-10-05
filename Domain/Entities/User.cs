using Domain.Enum.User.Types;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public required String FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public required UserStatus Status {  get; set; }
         public string? AvatarUrl { get; set; }
        public ICollection<ConversationUser>? ConversationUsers { get; set; }
        public ICollection<GroupUser>? GroupUsers { get; set; }
        public ICollection<NotificationUser>? NotificationUsers { get; set; }
        public ICollection<FriendRequest>? SentFriendRequests { get; set; }
        public ICollection<FriendRequest>? ReceivedFriendRequests { get; set; }
        public ICollection<SearchingHistory>? SearchingHistories { get; set; }
        public ICollection<SearchingHistory>? Searched { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<UserRelation>? Relations { get; set; }
        public ICollection<UserRelation>? RelatedTo { get; set; }
        public ICollection<Message>? MessageSent { get; set; }
        public ICollection<Message>? MessageReceive { get; set; }


    }
}
