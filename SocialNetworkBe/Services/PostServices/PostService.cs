using AutoMapper;
using Domain.Contracts.Requests.Post;
using Domain.Contracts.Responses.Notification;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Notification.Types;
using Domain.Enum.Post.Functions;
using Domain.Enum.Post.Types;
using Domain.Interfaces.BuilderInterfaces;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace SocialNetworkBe.Services.PostServices
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PostService> _logger;
        private readonly IUploadService _uploadService;
        private readonly INotificationDataBuilder _notificationDataBuilder;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public PostService(IUnitOfWork unitOfWork, ILogger<PostService> logger, IUploadService uploadService, INotificationDataBuilder notificationDataBuilder, IServiceProvider serviceProvider, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _uploadService = uploadService;
            _notificationDataBuilder = notificationDataBuilder;
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }

        public async Task<(CreatePostEnum, Guid?)> CreatePostAsync(CreatePostRequest request, Guid userId)
        {
            try
            {   
                var feedService = _serviceProvider.GetRequiredService<IFeedService>();
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return (CreatePostEnum.UserNotFound, null);
                }
              
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
                    // Tạo feed cho user khác
                    feedService.FeedForPost(post.Id, userId);
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
                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                    p => true,
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

                var postDtos = _mapper.Map<List<PostDto>>(sortedPosts);
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
                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
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

                var postDto = _mapper.Map<PostDto>(post);

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
                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
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
                    var postDto = _mapper.Map<PostDto>(post);
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

        public async Task<PostDto?> AddUpdateDeleteReactionPost(ReactionPostRequest request, Guid userId)
        {
            var notificationService = _serviceProvider.GetRequiredService<INotificationService>();
            try
            {
                // Tìm reaction hiện có của user cho post
                PostReactionUser? postReactionUser = await _unitOfWork.PostReactionUserRepository
                    .FindFirstAsync(r => r.PostId == request.PostId && r.UserId == userId);

                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                    p => p.Id == request.PostId,
                    p => p.User,
                    p => p.PostImages
                );

                var post = posts?.FirstOrDefault();
                if (post == null) return null;

                // Tìm chủ của post
                User? owner = post.User;
                // Tìm actor của post
                User? actor = await _unitOfWork.UserRepository.FindFirstAsync(u => u.Id == userId);

                if (postReactionUser == null)
                {
                    postReactionUser = new PostReactionUser
                    {
                        UserId = userId,
                        Reaction = request.Reaction,
                        PostId = request.PostId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    };

                    _unitOfWork.PostReactionUserRepository.Add(postReactionUser);
                    post.TotalLiked += 1;
                    if (userId != post.UserId) {
                        // Tạo data cho notification
                        NotificationData? notiData = await _notificationDataBuilder.BuilderDataForReactPost(post, actor, null);
                        string mergeKey = NotificationType.LikePost.ToString() + "_" + post.Id.ToString() + "_" + owner.Id.ToString();
                        string navigateUrl = $"/post/{post.Id}";
                        await notificationService.ProcessAndSendNotiForReactPost(NotificationType.LikePost, notiData, navigateUrl, mergeKey, owner.Id);                             
                    }
                }
                else if (postReactionUser.Reaction == request.Reaction)
                {                 
                    _unitOfWork.PostReactionUserRepository.Remove(postReactionUser);                 
                    post.TotalLiked = Math.Max(0, post.TotalLiked - 1);
                }
                else
                {                  
                    postReactionUser.Reaction = request.Reaction;
                    postReactionUser.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.PostReactionUserRepository.Update(postReactionUser);
                    if (userId != post.UserId)
                    {
                        // Tạo data cho notification
                        NotificationData? notiData = await _notificationDataBuilder.BuilderDataForReactPost(post, actor, null);
                        string mergeKey = NotificationType.LikePost.ToString() + "_" + post.Id.ToString() + "_" + owner.Id.ToString();
                        string navigateUrl = $"/post/{post.Id}";
                        await notificationService.ProcessAndSendNotiForReactPost(NotificationType.LikePost, notiData, navigateUrl, mergeKey, owner.Id);
                    }
                }

                _unitOfWork.PostRepository.Update(post);
                await _unitOfWork.CompleteAsync();

                var postDto = _mapper.Map<PostDto>(post);
                return postDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reacting to post {PostId}", request.PostId);
                return null;
            }
        }

        public async Task<(GetPostsByUserEnum, List<PostDto>?)> GetPostsByUserIdAsync(Guid userId, int skip = 0, int take = 10)
        {
            try
            {
                // Lấy tất cả posts của user với includes cho User và PostImages          
                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                    p => p.UserId == userId,
                    p => p.User,
                    p => p.PostImages
                );

                if (posts == null || !posts.Any())
                {
                    return (GetPostsByUserEnum.NoPostsFound, null);
                }

                // Sắp xếp theo thời gian tạo mới nhất và áp dụng pagination
                var sortedPosts = posts
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                var postDtos = _mapper.Map<List<PostDto>>(sortedPosts);

                return (GetPostsByUserEnum.Success, postDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting posts for user {UserId}", userId);
                return (GetPostsByUserEnum.Failed, null);
            }
        }
    }
}