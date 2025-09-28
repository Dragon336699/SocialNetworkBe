using Domain.Enum.Conversation;
using Domain.Enum.Group;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class GroupUser
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid GroupId { get; set; }
        public Group? Group { get; set; }
        [Required]
        public DateTime JoinedAt { get; set; }
        [Required]
        public GroupRole RoleName { get; set; }
    }
}
