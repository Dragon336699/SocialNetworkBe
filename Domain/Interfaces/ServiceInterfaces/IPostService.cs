using Domain.Contracts.Requests.Post;
using Domain.Contracts.Responses.Post;
using Domain.Enum.Post.Functions;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IPostService
    {
        Task<(CreatePostEnum, Guid?)> CreatePostAsync(CreatePostRequest request, Guid userId);
        Task<(GetAllPostsEnum, List<PostDto>?)> GetAllPostsAsync(int skip = 0, int take = 10);
    }
}