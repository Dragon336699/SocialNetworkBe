namespace Domain.Entities.NoSQL
{
    public class UserFeedSeen
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid FeedId { get; set; }
        public DateTime SeenAt { get; set; }
    }
}
