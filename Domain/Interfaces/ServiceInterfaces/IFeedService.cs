using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.Post.UserFeed;
using Domain.Entities;
using Domain.Enum.Post.Functions;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IFeedService
    {
        Task FeedForPost(Guid postId, Guid authorId);
        Task<(GetAllPostsEnum, List<PostDto>)> GetFeedsForUser (Guid userId);
    }
}
