using Domain.Contracts.Responses.Feed;
using Domain.Enum.Post.Functions;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IFeedService
    {
        Task FeedForPost(Guid postId, Guid authorId);
        Task<(GetAllPostsEnum, List<FeedDto>)> GetFeedsForUser(Guid userId);
    }
}
