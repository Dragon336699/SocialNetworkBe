using Domain.Contracts.Requests.Post;
using Domain.Contracts.Responses.Feed;
using Domain.Enum.Post.Functions;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IFeedService
    {
        Task FeedForPost(Guid postId, Guid authorId);
        Task<(GetAllPostsEnum, List<FeedDto>)> GetFeedsForUser(Guid userId);
        void SeenFeed(List<SeenFeedRequest> request, Guid userId);
    }
}
