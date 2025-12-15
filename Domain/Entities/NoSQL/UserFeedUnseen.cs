namespace Domain.Entities.NoSQL
{
    public class UserFeedUnseen
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid FeedId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
