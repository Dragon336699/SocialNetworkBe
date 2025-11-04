using Domain.Contracts.Requests.Post;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Post.Functions;
using Domain.Enum.Post.Types;
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

        public async Task<(GetPostByIdEnum, PostDto?)> GetPostByIdAsync(Guid postId, Guid userId)
        {
            try
            {
                // Lấy post với includes
                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludes(
                    p => p.Id == postId,
                    p => p.User,
                    p => p.PostImages
                );

                var post = posts?.FirstOrDefault();
                if (post == null)
                {
                    return (GetPostByIdEnum.PostNotFound, null);
                }

                // Kiểm tra quyền xem post          
                if (post.UserId != userId)
                {
                    // Kiểm tra privacy settings
                    switch (post.PostPrivacy)
                    {
                        case PostPrivacy.Private:
                            return (GetPostByIdEnum.Unauthorized, null);
                        case PostPrivacy.Friends:                    
                            break;
                        case PostPrivacy.Public:                          
                            break;
                    }
                }

                // Tạo PostDto
                var postDto = new PostDto
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
                };

                return (GetPostByIdEnum.Success, postDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting post {PostId} for user {UserId}", postId, userId);
                return (GetPostByIdEnum.Failed, null);
            }
        }

        public async Task<(UpdatePostEnum, PostDto?)> UpdatePostAsync(Guid postId, UpdatePostRequest request, Guid userId)
        {
            try
            {               
                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludes(
                    p => p.Id == postId,
                    p => p.User,
                    p => p.PostImages
                );

                var post = posts?.FirstOrDefault();
                if (post == null)
                {
                    return (UpdatePostEnum.PostNotFound, null);
                }
               
                if (post.UserId != userId)
                {
                    return (UpdatePostEnum.Unauthorized, null);
                }
                
                if (request.Content != null)
                {
                    if (string.IsNullOrWhiteSpace(request.Content))
                    {
                        return (UpdatePostEnum.InvalidContent, null);
                    }
                    post.Content = request.Content.Trim();
                }
              
                if (request.PostPrivacy.HasValue)
                {
                    post.PostPrivacy = request.PostPrivacy.Value;
                }

                // Xử lý xóa hình ảnh
                if (request.RemoveAllImages && post.PostImages != null)
                {                 
                    post.PostImages.Clear();
                }
                else if (request.ImageIdsToDelete != null && request.ImageIdsToDelete.Any())
                {                 
                    var imagesToDelete = post.PostImages?
                        .Where(img => request.ImageIdsToDelete.Contains(img.Id))
                        .ToList();

                    if (imagesToDelete != null)
                    {
                        foreach (var image in imagesToDelete)
                        {
                            post.PostImages.Remove(image);
                        }
                    }
                }

                // Xử lý upload hình ảnh mới
                if (request.NewImages != null && request.NewImages.Any())
                {
                    // Validate file types
                    var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var invalidFiles = request.NewImages.Where(file =>
                        !validImageExtensions.Any(ext =>
                            file.FileName.ToLower().EndsWith(ext))).ToList();

                    if (invalidFiles.Any())
                    {
                        return (UpdatePostEnum.InvalidImageFormat, null);
                    }

                    // Validate file sizes
                    const long maxFileSize = 10 * 1024 * 1024;
                    var oversizedFiles = request.NewImages.Where(file => file.Length > maxFileSize).ToList();

                    if (oversizedFiles.Any())
                    {
                        return (UpdatePostEnum.FileTooLarge, null);
                    }

                    // Upload new images
                    var newImageUrls = await _uploadService.UploadFile(request.NewImages, "posts/images");
                    if (newImageUrls == null || !newImageUrls.Any())
                    {
                        return (UpdatePostEnum.ImageUploadFailed, null);
                    }
                
                    var newPostImages = newImageUrls
                        .Where(url => !string.IsNullOrWhiteSpace(url))
                        .Select(imageUrl => new PostImage
                        {
                            PostId = post.Id,
                            ImageUrl = imageUrl
                        }).ToList();

                    if (post.PostImages == null)
                    {
                        post.PostImages = new List<PostImage>();
                    }

                    foreach (var newImage in newPostImages)
                    {
                        post.PostImages.Add(newImage);
                    }
                }
  
                post.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.PostRepository.Update(post);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {                  
                    var postDto = new PostDto
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
                    };

                    return (UpdatePostEnum.UpdatePostSuccess, postDto);
                }

                return (UpdatePostEnum.UpdatePostFailed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating post {PostId} for user {UserId}", postId, userId);
                return (UpdatePostEnum.UpdatePostFailed, null);
            }
        }

        public async Task<(DeletePostEnum, bool)> DeletePostAsync(Guid postId, Guid userId)
        {
            try
            {             
                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludes(
                    p => p.Id == postId,
                    p => p.User,
                    p => p.PostImages
                );

                var post = posts?.FirstOrDefault();
                if (post == null)
                {
                    return (DeletePostEnum.PostNotFound, false);
                }
          
                if (post.UserId != userId)
                {
                    return (DeletePostEnum.Unauthorized, false);
                }
              
                _unitOfWork.PostRepository.Remove(post);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    return (DeletePostEnum.DeletePostSuccess, true);
                }

                return (DeletePostEnum.DeletePostFailed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting post {PostId} for user {UserId}", postId, userId);
                return (DeletePostEnum.DeletePostFailed, false);
            }
        }
    }
}