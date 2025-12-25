namespace Domain.Entities.NoSQL
{
    public class UserInteractionCounter
    {
        public Guid UserId { get; set; }
        public Guid TargetUserId { get; set; }
        public int ViewCount { get; set; }
        public int SearchCount { get; set; }
        public int LikeCount { get; set; }
    }
}
