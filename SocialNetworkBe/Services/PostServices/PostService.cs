using AutoMapper;
using Domain.Contracts.Requests.Post;
using Domain.Contracts.Responses.Notification;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Group.Types;
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

        public async Task<(CreatePostEnum, PostDto?)> CreatePostAsync(CreatePostRequest request, Guid userId)
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
              
                PostPrivacy postPrivacy = request.PostPrivacy;
                bool needsApproval = false;
                
                if (request.GroupId.HasValue)
                {
                    var groupUser = await _unitOfWork.GroupUserRepository.FindFirstAsync(
                        gu => gu.GroupId == request.GroupId.Value && gu.UserId == userId
                    );
                                      
                    if (groupUser == null || groupUser.RoleName == GroupRole.Pending || groupUser.RoleName == GroupRole.Inviting)
                    {
                        return (CreatePostEnum.UserNotFound, null);
                    }
                                      
                    if (groupUser.RoleName != GroupRole.Administrator && groupUser.RoleName != GroupRole.SuperAdministrator)
                    {
                        postPrivacy = PostPrivacy.PendingApproval;
                        needsApproval = true;
                    }
                }

                List<string>? imageUrls = null;
                if (request.Images != null && request.Images.Any())
                {                   
                    var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var invalidFiles = request.Images.Where(file =>
                        !validImageExtensions.Any(ext =>
                            file.FileName.ToLower().EndsWith(ext))).ToList();

                    if (invalidFiles.Any())
                    {                       
                        return (CreatePostEnum.InvalidImageFormat, null);
                    }
                  
                    const long maxFileSize = 10 * 1024 * 1024;
                    var oversizedFiles = request.Images.Where(file => file.Length > maxFileSize).ToList();

                    if (oversizedFiles.Any())
                    {                       
                        return (CreatePostEnum.FileTooLarge, null);
                    }
                  
                    imageUrls = await _uploadService.UploadFile(request.Images, "posts/images");
                    if (imageUrls == null || !imageUrls.Any())
                    {                      
                        return (CreatePostEnum.ImageUploadFailed, null);
                    }
                }
                
                var post = new Post
                {
                    Content = request.Content.Trim(),
                    TotalLiked = 0,
                    TotalComment = 0,
                    PostPrivacy = postPrivacy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = userId,
                    GroupId = request.GroupId
                };               
              
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
                    if (!needsApproval)
                    {
                        await feedService.FeedForPost(post.Id, userId);
                    }
                    
                    var createdPosts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                        p => p.Id == post.Id,
                        p => p.User,
                        p => p.PostImages,
                        p => p.Group
                    );
                    
                    var createdPost = createdPosts?.FirstOrDefault();
                    var postDto = _mapper.Map<PostDto>(createdPost);
                    return (CreatePostEnum.CreatePostSuccess, postDto);
                }
                
                return (CreatePostEnum.CreatePostFailed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when creating post for user {UserId}", userId);
                return (CreatePostEnum.CreatePostFailed, null);
            }
        }

        public async Task<(GetAllPostsEnum, List<PostDto>?)> GetAllPostsAsync(Guid currentUserId, int skip = 0, int take = 10)
        {
            try
            {
                var blockedUserIds = await GetBlockedUserIdsAsync(currentUserId);

                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                    p => p.PostPrivacy != PostPrivacy.PendingApproval &&
                         p.GroupId == null &&
                         !blockedUserIds.Contains(p.UserId),
                    p => p.User,
                    p => p.PostImages
                );

                if (posts == null || !posts.Any())
                {
                    return (GetAllPostsEnum.NoPostsFound, null);
                }
           
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
                   
                if (post.UserId != userId)
                {                  
                    switch (post.PostPrivacy)
                    {
                        case PostPrivacy.Private:
                            return (GetPostByIdEnum.Unauthorized, null);
                        case PostPrivacy.PendingApproval:
                            return (GetPostByIdEnum.Unauthorized, null);
                        case PostPrivacy.Friends:                          
                            bool areFriends = await _unitOfWork.FriendRequestRepository.AreFriendsAsync(userId, post.UserId);
                            if (!areFriends)
                            {
                                return (GetPostByIdEnum.Unauthorized, null);
                            }
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
              
                if (request.NewImages != null && request.NewImages.Any())
                {                   
                    var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var invalidFiles = request.NewImages.Where(file =>
                        !validImageExtensions.Any(ext =>
                            file.FileName.ToLower().EndsWith(ext))).ToList();

                    if (invalidFiles.Any())
                    {
                        return (UpdatePostEnum.InvalidImageFormat, null);
                    }
                  
                    const long maxFileSize = 10 * 1024 * 1024;
                    var oversizedFiles = request.NewImages.Where(file => file.Length > maxFileSize).ToList();

                    if (oversizedFiles.Any())
                    {
                        return (UpdatePostEnum.FileTooLarge, null);
                    }
                  
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
                    
                    if (post.GroupId.HasValue)
                    {
                        var groupUser = await _unitOfWork.GroupUserRepository.FindFirstAsync(
                            gu => gu.GroupId == post.GroupId.Value && gu.UserId == userId
                        );
                        
                        if (groupUser == null ||
                            (groupUser.RoleName != GroupRole.Administrator &&
                             groupUser.RoleName != GroupRole.SuperAdministrator))
                        {
                            return (DeletePostEnum.Unauthorized, false);
                        }                      
                    }
                    else
                    {                     
                        return (DeletePostEnum.Unauthorized, false);
                    }
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
                PostReactionUser? postReactionUser = await _unitOfWork.PostReactionUserRepository
                    .FindFirstAsync(r => r.PostId == request.PostId && r.UserId == userId);

                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                    p => p.Id == request.PostId,
                    p => p.User,
                    p => p.PostImages
                );

                var post = posts?.FirstOrDefault();
                if (post == null) return null;

               
                User? owner = post.User;                
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

        public async Task<(GetPostsByUserEnum, List<PostDto>?)> GetPostsByUserIdAsync(Guid targetUserId, Guid currentUserId, int skip = 0, int take = 10)
        {
            try
            {
                var blockedUserIds = await GetBlockedUserIdsAsync(currentUserId);
                if (blockedUserIds.Contains(targetUserId))
                {
                    return (GetPostsByUserEnum.NoPostsFound, null);
                }

                IEnumerable<Post> posts;
                
                if (targetUserId == currentUserId)
                {
                    
                    posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                        p => p.UserId == targetUserId && 
                             p.PostPrivacy != PostPrivacy.PendingApproval &&
                             p.GroupId == null,
                        p => p.User,
                        p => p.PostImages
                    );
                }
                else
                {
                    
                    bool areFriends = await _unitOfWork.FriendRequestRepository.AreFriendsAsync(currentUserId, targetUserId);                   
                    if (areFriends)
                    {                       
                        posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                            p => p.UserId == targetUserId && 
                                 (p.PostPrivacy == PostPrivacy.Public || p.PostPrivacy == PostPrivacy.Friends) &&
                                 p.GroupId == null,
                            p => p.User,
                            p => p.PostImages
                        );
                    }
                    else
                    {                  
                        posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                            p => p.UserId == targetUserId && 
                                 p.PostPrivacy == PostPrivacy.Public &&
                                 p.GroupId == null,
                            p => p.User,
                            p => p.PostImages
                        );
                    }
                }

                if (posts == null || !posts.Any())
                {
                    return (GetPostsByUserEnum.NoPostsFound, null);
                }
              
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
                _logger.LogError(ex, "Error when getting posts for user {UserId}", targetUserId);
                return (GetPostsByUserEnum.Failed, null);
            }
        }

        public async Task<(GetPendingPostsEnum, List<PostDto>?)> GetPendingPostsAsync(Guid groupId, Guid currentUserId, int skip = 0, int take = 10)
        {
            try
            {              
                var groups = await _unitOfWork.GroupRepository.FindAsyncWithIncludes(
                    g => g.Id == groupId,
                    g => g.GroupUsers
                );
                
                var group = groups?.FirstOrDefault();
                if (group == null)
                {
                    return (GetPendingPostsEnum.GroupNotFound, null);
                }
                               
                var currentUserRole = group.GroupUsers?.FirstOrDefault(gu => gu.UserId == currentUserId);
                if (currentUserRole == null ||
                    (currentUserRole.RoleName != GroupRole.SuperAdministrator &&
                     currentUserRole.RoleName != GroupRole.Administrator))
                {
                    return (GetPendingPostsEnum.Unauthorized, null);
                }
                               
                var pendingPosts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                    p => p.GroupId == groupId && p.PostPrivacy == PostPrivacy.PendingApproval,
                    p => p.User,
                    p => p.PostImages
                );
                
                if (pendingPosts == null || !pendingPosts.Any())
                {
                    return (GetPendingPostsEnum.NoPendingPosts, null);
                }
                
                var sortedPosts = pendingPosts
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
                
                var postDtos = _mapper.Map<List<PostDto>>(sortedPosts);
                
                return (GetPendingPostsEnum.Success, postDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting pending posts for group {GroupId}", groupId);
                return (GetPendingPostsEnum.Failed, null);
            }
        }

        public async Task<(ApprovePostEnum, PostDto?)> ApprovePostAsync(Guid postId, Guid currentUserId)
        {
            try
            {
                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                    p => p.Id == postId,
                    p => p.User,
                    p => p.PostImages,
                    p => p.Group
                );
                
                var post = posts?.FirstOrDefault();
                if (post == null)
                {
                    return (ApprovePostEnum.PostNotFound, null);
                }
                
                if (post.PostPrivacy != PostPrivacy.PendingApproval)
                {
                    return (ApprovePostEnum.PostNotPending, null);
                }
                
                if (!post.GroupId.HasValue)
                {
                    return (ApprovePostEnum.GroupNotFound, null);
                }
                               
                var groupUser = await _unitOfWork.GroupUserRepository.FindFirstAsync(
                    gu => gu.GroupId == post.GroupId.Value && gu.UserId == currentUserId
                );
                
                if (groupUser == null ||
                    (groupUser.RoleName != GroupRole.SuperAdministrator &&
                     groupUser.RoleName != GroupRole.Administrator))
                {
                    return (ApprovePostEnum.Unauthorized, null);
                }
                             
                post.PostPrivacy = PostPrivacy.Public;
                post.UpdatedAt = DateTime.UtcNow;
                
                _unitOfWork.PostRepository.Update(post);
                var result = await _unitOfWork.CompleteAsync();
                
                if (result > 0)
                {                
                    var feedService = _serviceProvider.GetRequiredService<IFeedService>();
                    await feedService.FeedForPost(post.Id, post.UserId);
                    
                    var postDto = _mapper.Map<PostDto>(post);
                    return (ApprovePostEnum.Success, postDto);
                }
                
                return (ApprovePostEnum.Failed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when approving post {PostId}", postId);
                return (ApprovePostEnum.Failed, null);
            }
        }

        public async Task<(RejectPostEnum, bool)> RejectPostAsync(Guid postId, Guid currentUserId)
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
                    return (RejectPostEnum.PostNotFound, false);
                }
                
                if (post.PostPrivacy != PostPrivacy.PendingApproval)
                {
                    return (RejectPostEnum.PostNotPending, false);
                }
                
                if (!post.GroupId.HasValue)
                {
                    return (RejectPostEnum.GroupNotFound, false);
                }
                               
                var groupUser = await _unitOfWork.GroupUserRepository.FindFirstAsync(
                    gu => gu.GroupId == post.GroupId.Value && gu.UserId == currentUserId
                );
                
                if (groupUser == null ||
                    (groupUser.RoleName != GroupRole.SuperAdministrator &&
                     groupUser.RoleName != GroupRole.Administrator))
                {
                    return (RejectPostEnum.Unauthorized, false);
                }
                _unitOfWork.PostRepository.Remove(post);
                var result = await _unitOfWork.CompleteAsync();
                
                if (result > 0)
                {
                    return (RejectPostEnum.Success, true);
                }
                
                return (RejectPostEnum.Failed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when rejecting post {PostId}", postId);
                return (RejectPostEnum.Failed, false);
            }
        }

        public async Task<(GetMyPendingPostsEnum, List<PostDto>?)> GetMyPendingPostsAsync(Guid userId, Guid? groupId = null, int skip = 0, int take = 10)
        {
            try
            {
                IEnumerable<Post> pendingPosts;
                
                if (groupId.HasValue)
                {                   
                    pendingPosts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                        p => p.UserId == userId && 
                             p.GroupId == groupId.Value && 
                             p.PostPrivacy == PostPrivacy.PendingApproval,
                        p => p.User,
                        p => p.PostImages,
                        p => p.Group
                    );
                }
                else
                {                   
                    pendingPosts = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                        p => p.UserId == userId && p.PostPrivacy == PostPrivacy.PendingApproval,
                        p => p.User,
                        p => p.PostImages,
                        p => p.Group
                    );
                }
                
                if (pendingPosts == null || !pendingPosts.Any())
                {
                    return (GetMyPendingPostsEnum.NoPendingPosts, null);
                }
                
                var sortedPosts = pendingPosts
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
                
                var postDtos = _mapper.Map<List<PostDto>>(sortedPosts);
                
                return (GetMyPendingPostsEnum.Success, postDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting pending posts for user {UserId}", userId);
                return (GetMyPendingPostsEnum.Failed, null);
            }
        }

        public async Task<(CancelPendingPostEnum, bool)> CancelPendingPostAsync(Guid postId, Guid userId)
        {
            try
            {
                var posts = await _unitOfWork.PostRepository.FindAsyncWithIncludes(
                    p => p.Id == postId,
                    p => p.PostImages
                );
                
                var post = posts?.FirstOrDefault();
                if (post == null)
                {
                    return (CancelPendingPostEnum.PostNotFound, false);
                }
                
                if (post.PostPrivacy != PostPrivacy.PendingApproval)
                {
                    return (CancelPendingPostEnum.PostNotPending, false);
                }
                                
                if (post.UserId != userId)
                {
                    return (CancelPendingPostEnum.Unauthorized, false);
                }
                              
                _unitOfWork.PostRepository.Remove(post);
                var result = await _unitOfWork.CompleteAsync();
                
                if (result > 0)
                {
                    return (CancelPendingPostEnum.Success, true);
                }
                
                return (CancelPendingPostEnum.Failed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when cancelling pending post {PostId}", postId);
                return (CancelPendingPostEnum.Failed, false);
            }
        }

        private async Task<List<Guid>> GetBlockedUserIdsAsync(Guid userId)
        {
            try
            {          
                var blockedByMe = await _unitOfWork.UserRelationRepository.FindAsync(
                    ur => ur.UserId == userId && ur.RelationType == Domain.Enum.User.Types.UserRelationType.Blocked
                );
             
                var blockedMe = await _unitOfWork.UserRelationRepository.FindAsync(
                    ur => ur.RelatedUserId == userId && ur.RelationType == Domain.Enum.User.Types.UserRelationType.Blocked
                );

                var blockedIds = new List<Guid>();

                if (blockedByMe != null && blockedByMe.Any())
                {
                    blockedIds.AddRange(blockedByMe.Select(ur => ur.RelatedUserId));
                }

                if (blockedMe != null && blockedMe.Any())
                {
                    blockedIds.AddRange(blockedMe.Select(ur => ur.UserId));
                }

                return blockedIds.Distinct().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blocked users for user {UserId}", userId);
                return new List<Guid>();
            }
        }
    }
}