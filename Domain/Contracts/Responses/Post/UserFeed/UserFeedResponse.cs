using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Contracts.Responses.Post.UserFeed
{
    public class UserFeedResponse
    {
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid AuthorId { get; set; }
        public Guid PostId { get; set; }
        public DateTime SeenAt { get; set; }
    }
}
