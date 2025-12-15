using Domain.Entities.NoSQL;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IFeedRepository
    {
        void FeedForPost(Guid postId, List<Guid> userIds, Guid authorId);
        Task<List<UserFeedUnseen>> GetFeedsForUser(Guid userId);
    }
}
