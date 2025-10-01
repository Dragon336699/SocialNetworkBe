using Domain.Enum.User.Types;

namespace Domain.Entities
{
    public class UserRelation
    {
        public Guid UserId { get; set; }
        public Guid RelatedUserId { get; set; }
        public UserRelationType RelationType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User? User { get; set; }
        public User? RelatedUser { get; set; }
    }
}
