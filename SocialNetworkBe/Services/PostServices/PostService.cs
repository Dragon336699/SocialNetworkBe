using Domain.Contracts.Requests.Post;
using Domain.Entities;
using Domain.Enum.Post.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace SocialNetworkBe.Services.PostServices
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PostService> _logger;      

        public PostService(IUnitOfWork unitOfWork, ILogger<PostService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<(CreatePostEnum, Guid?)> CreatePostAsync(CreatePostRequest request, Guid userId)
        {
            try
            {
                // Kiểm tra user tồn tại
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return (CreatePostEnum.UserNotFound, null);
                }

                // Validate content
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return (CreatePostEnum.InvalidContent, null);
                }

                // Tạo post
                var post = new Post
                {
                    Content = request.Content.Trim(),
                    TotalLiked = 0,
                    TotalComment = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = userId
                };
                _unitOfWork.PostRepository.Add(post);

                // Thêm hình ảnh nếu có
                if (request.ImageUrls != null && request.ImageUrls.Any())
                {
                    var postImages = request.ImageUrls
                        .Where(url => !string.IsNullOrWhiteSpace(url))
                        .Select(imageUrl => new PostImage
                        {
                            PostId = post.Id,
                            ImageUrl = imageUrl
                        }).ToList();
                    _unitOfWork.PostImageRepository.AddRange(postImages);
                }

                var result = await _unitOfWork.CompleteAsync();
                if (result > 0)
                {
                    return (CreatePostEnum.CreatePostSuccess, post.Id);
                }

                return (CreatePostEnum.CreatePostFailed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when creating post for user {UserId}", userId);
                return (CreatePostEnum.CreatePostFailed, null);
            }
        }
    }
}