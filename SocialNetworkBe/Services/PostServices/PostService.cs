using Domain.Contracts.Requests.Post;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.User;
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
        private readonly IUploadService _uploadService;

        public PostService(IUnitOfWork unitOfWork, ILogger<PostService> logger, IUploadService uploadService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _uploadService = uploadService;
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

                // Upload images lên cloud trước khi tạo post
                List<string>? imageUrls = null;
                if (request.Images != null && request.Images.Any())
                {
                    // Validate file types (chỉ cho phép images)
                    var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var invalidFiles = request.Images.Where(file =>
                        !validImageExtensions.Any(ext =>
                            file.FileName.ToLower().EndsWith(ext))).ToList();

                    if (invalidFiles.Any())
                    {                       
                        return (CreatePostEnum.InvalidImageFormat, null);
                    }

                    // Validate file sizes (max 10MB per file)
                    const long maxFileSize = 10 * 1024 * 1024;
                    var oversizedFiles = request.Images.Where(file => file.Length > maxFileSize).ToList();

                    if (oversizedFiles.Any())
                    {                       
                        return (CreatePostEnum.FileTooLarge, null);
                    }

                    // Upload files to Cloudinary
                    imageUrls = await _uploadService.UploadFile(request.Images, "posts/images");
                    if (imageUrls == null || !imageUrls.Any())
                    {                      
                        return (CreatePostEnum.ImageUploadFailed, null);
                    }
                }

                // Tạo post với chế độ riêng tư 
                var post = new Post
                {
                    Content = request.Content.Trim(),
                    TotalLiked = 0,
                    TotalComment = 0,
                    PostPrivacy = request.PostPrivacy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = userId,
                    GroupId = request.GroupId
                };               

                // Thêm hình ảnh với URLs từ cloud
                if (imageUrls != null && imageUrls.Any())
                {
                    post.PostImages = imageUrls
                        .Where(url => !string.IsNullOrWhiteSpace(url))
                        .Select(imageUrl => new PostImage
                        {
                            ImageUrl = imageUrl                           
                        }).ToList();
                }
                _unitOfWork.PostRepository.Add(post);
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

        public async Task<(GetAllPostsEnum, List<PostDto>?)> GetAllPostsAsync(int skip = 0, int take = 10)
        {
            try
            {
                // Lấy tất cả posts với includes cho User và PostImages
                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludes(
                    p => true, // Lấy tất cả posts
                    p => p.User,
                    p => p.PostImages
                );

                if (posts == null || !posts.Any())
                {
                    return (GetAllPostsEnum.NoPostsFound, null);
                }

                // Sắp xếp theo thời gian tạo mới nhất và áp dụng pagination
                var sortedPosts = posts
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                var postDtos = sortedPosts.Select(post => new PostDto
                {
                    Id = post.Id,
                    Content = post.Content,
                    TotalLiked = post.TotalLiked,
                    TotalComment = post.TotalComment,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    PostPrivacy = post.PostPrivacy,
                    UserId = post.UserId,
                    GroupId = post.GroupId,
                    User = post.User == null ? null : new UserDto
                    {
                        Id = post.User.Id,
                        Email = post.User.Email,
                        UserName = post.User.UserName ?? "",
                        Status = post.User.Status.ToString(),
                        FirstName = post.User.FirstName,
                        LastName = post.User.LastName,
                        AvatarUrl = post.User.AvatarUrl
                    },
                    PostImages = post.PostImages?.Select(img => new PostImageDto
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl
                    }).ToList()
                }).ToList();

                return (GetAllPostsEnum.Success, postDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting all posts");
                return (GetAllPostsEnum.Failed, null);
            }
        }
    }
}