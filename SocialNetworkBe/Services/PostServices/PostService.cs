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
                //List<string>? imageUrls = null;
                //if (request.Images != null && request.Images.Any())
                //{
                //    // Validate file types (chỉ cho phép images)
                //    var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                //    var invalidFiles = request.Images.Where(file =>
                //        !validImageExtensions.Any(ext =>
                //            file.FileName.ToLower().EndsWith(ext))).ToList();

                //    if (invalidFiles.Any())
                //    {
                //        _logger.LogWarning("Invalid image files detected: {FileNames}",
                //            string.Join(", ", invalidFiles.Select(f => f.FileName)));
                //        return (CreatePostEnum.InvalidImageFormat, null);
                //    }

                //    // Validate file sizes (max 10MB per file)
                //    const long maxFileSize = 10 * 1024 * 1024;
                //    var oversizedFiles = request.Images.Where(file => file.Length > maxFileSize).ToList();

                //    if (oversizedFiles.Any())
                //    {
                //        _logger.LogWarning("Files too large detected: {FileNames}",
                //            string.Join(", ", oversizedFiles.Select(f => f.FileName)));
                //        return (CreatePostEnum.FileTooLarge, null);
                //    }

                //    // Upload files to Cloudinary
                //    imageUrls = await _uploadService.UploadFile(request.Images, "posts/images");
                //    if (imageUrls == null || !imageUrls.Any())
                //    {
                //        _logger.LogError("Failed to upload images to cloud storage");
                //        return (CreatePostEnum.ImageUploadFailed, null);
                //    }
                //}

                // Tạo post với chế độ riêng tư được chỉ định
                var post = new Post
                {
                    Content = request.Content.Trim(),
                    TotalLiked = 0,
                    TotalComment = 0,
                    //PostPrivacy = request.PostPrivacy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = userId
                };
                _unitOfWork.PostRepository.Add(post);

                // Thêm hình ảnh với URLs từ cloud
                //if (imageUrls != null && imageUrls.Any())
                //{
                //    var postImages = imageUrls
                //        .Where(url => !string.IsNullOrWhiteSpace(url))
                //        .Select(imageUrl => new PostImage
                //        {
                //            PostId = post.Id,
                //            ImageUrl = imageUrl
                //        }).ToList();
                //    _unitOfWork.PostImageRepository.AddRange(postImages);
                //}

                //var result = await _unitOfWork.CompleteAsync();
                //if (result > 0)
                //{
                //    _logger.LogInformation("Post created successfully with privacy {PostPrivacy}, {ImageCount} images for user {UserId}",
                //        request.PostPrivacy, imageUrls?.Count ?? 0, userId);
                //    return (CreatePostEnum.CreatePostSuccess, post.Id);
                //}

                var result = await _unitOfWork.CompleteAsync();
                if (result > 0)
                {
                    _logger.LogInformation("Post created successfully with privacy {PostPrivacy}, {ImageCount} images for user {UserId}",
                        request.PostPrivacy, userId);
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