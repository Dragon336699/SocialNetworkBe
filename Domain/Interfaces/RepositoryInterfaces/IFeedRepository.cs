using Domain.Contracts.Requests.Post;
using Domain.Entities.NoSQL;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IFeedRepository
    {
        void FeedForPost(Guid postId, List<Guid> userIds);
        Task<List<UserFeedUnseen>> GetFeedsForUser(Guid userId);
        void SeenFeed(List<SeenFeedRequest> request, Guid userId);
    }
}
