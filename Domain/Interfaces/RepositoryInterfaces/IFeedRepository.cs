using Domain.Contracts.Requests.Post;
using Domain.Entities.NoSQL;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IFeedRepository
    {
        Task FeedForPost(Guid postId, List<Guid> userIds, Guid authorId);
        Task<List<UserFeedUnseen>> GetFeedsForUser(Guid userId);
        void SeenFeed(List<SeenFeedRequest> request, Guid userId);
    }
}
