namespace Domain.Entities.NoSQL
{
    public class UserFeedUnseen
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public long CreatedAt { get; set; }
    }
}
