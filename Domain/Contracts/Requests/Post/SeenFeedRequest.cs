namespace Domain.Contracts.Requests.Post
{
    public class SeenFeedRequest
    {
        public Guid FeedId { get; set; }
        public Guid PostId { get; set; }
        public long CreatedAt { get; set; }
    }
}
