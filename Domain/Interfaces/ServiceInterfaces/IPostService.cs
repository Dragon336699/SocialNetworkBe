using Domain.Contracts.Requests.Post;
using Domain.Enum.Post.Functions;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IPostService
    {
        Task<(CreatePostEnum, Guid?)> CreatePostAsync(CreatePostRequest request, Guid userId);
    }
}