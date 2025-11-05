using Domain.Contracts.Requests.Post;
using Domain.Contracts.Responses.Post;
using Domain.Enum.Post.Functions;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IPostService
    {
        Task<(CreatePostEnum, Guid?)> CreatePostAsync(CreatePostRequest request, Guid userId);
        Task<(GetAllPostsEnum, List<PostDto>?)> GetAllPostsAsync(int skip = 0, int take = 10);
        Task<(GetPostByIdEnum, PostDto?)> GetPostByIdAsync(Guid postId, Guid userId);
        Task<(UpdatePostEnum, PostDto?)> UpdatePostAsync(Guid postId, UpdatePostRequest request, Guid userId);
        Task<(DeletePostEnum, bool)> DeletePostAsync(Guid postId, Guid userId);
        Task<PostDto?> AddUpdateDeleteReactionPost(ReactionPostRequest request, Guid userId);
    }
}