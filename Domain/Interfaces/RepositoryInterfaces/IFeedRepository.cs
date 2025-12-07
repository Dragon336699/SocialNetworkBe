using Domain.Contracts.Responses.Post.UserFeed;
using Domain.Entities;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IFeedRepository
    {
        void FeedForPost(Guid postId, List<Guid> userIds, Guid authorId);
        Task<List<UserFeedResponse>> GetFeedsForUser(Guid userId);
    }
}
