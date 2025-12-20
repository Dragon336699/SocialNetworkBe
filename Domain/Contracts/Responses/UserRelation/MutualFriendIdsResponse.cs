namespace Domain.Contracts.Responses.UserRelation
{
    public class MutualFriendIdsResponse
    {
        public Guid SuggestedUserId { get; set; }
        public int MutualFriendCount { get; set; }
    }
}
