using Domain.Contracts.Requests.Post;
using Domain.Contracts.Responses.Post;
using Domain.Enum.Post.Functions;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IPostService
    {
        Task<(CreatePostEnum, PostDto?)> CreatePostAsync(CreatePostRequest request, Guid userId);
        Task<(GetAllPostsEnum, List<PostDto>?)> GetAllPostsAsync(Guid currentUserId, int skip = 0, int take = 10);
        Task<(GetPostByIdEnum, PostDto?)> GetPostByIdAsync(Guid postId, Guid userId);
        Task<(UpdatePostEnum, PostDto?)> UpdatePostAsync(Guid postId, UpdatePostRequest request, Guid userId);
        Task<(DeletePostEnum, bool)> DeletePostAsync(Guid postId, Guid userId);
        Task<PostDto?> AddUpdateDeleteReactionPost(ReactionPostRequest request, Guid userId);
        Task<(GetPostsByUserEnum, List<PostDto>?)> GetPostsByUserIdAsync(Guid targetUserId, Guid currentUserId, int skip = 0, int take = 10);
        Task<(GetPendingPostsEnum, List<PostDto>?)> GetPendingPostsAsync(Guid groupId, Guid currentUserId, int skip = 0, int take = 10);
        Task<(ApprovePostEnum, PostDto?)> ApprovePostAsync(Guid postId, Guid currentUserId);
        Task<(RejectPostEnum, bool)> RejectPostAsync(Guid postId, Guid currentUserId);
        Task<(GetMyPendingPostsEnum, List<PostDto>?)> GetMyPendingPostsAsync(Guid userId, Guid? groupId = null, int skip = 0, int take = 10);
        Task<(CancelPendingPostEnum, bool)> CancelPendingPostAsync(Guid postId, Guid userId);
    }
}