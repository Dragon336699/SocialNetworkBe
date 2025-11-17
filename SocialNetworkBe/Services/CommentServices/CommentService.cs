using Domain.Contracts.Requests.Comment;
using Domain.Contracts.Responses.Comment;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Comment.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace SocialNetworkBe.Services.CommentServices
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CommentService> _logger;
        private readonly IUploadService _uploadService;

        public CommentService(IUnitOfWork unitOfWork, ILogger<CommentService> logger, IUploadService uploadService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _uploadService = uploadService;
        }

        public async Task<(CreateCommentEnum, Guid?)> CreateCommentAsync(CreateCommentRequest request, Guid userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return (CreateCommentEnum.UserNotFound, null);
                }
                
                var post = await _unitOfWork.PostRepository.GetByIdAsync(request.PostId);
                if (post == null)
                {
                    return (CreateCommentEnum.PostNotFound, null);
                }
               
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return (CreateCommentEnum.InvalidContent, null);
                }

                // Kiểm tra parent comment nếu là reply
                if (request.RepliedCommentId.HasValue)
                {
                    var parentComment = await _unitOfWork.CommentRepository.GetByIdAsync(request.RepliedCommentId.Value);
                    if (parentComment == null)
                    {
                        return (CreateCommentEnum.ParentCommentNotFound, null);
                    }
                }

                // Upload images 
                List<string>? imageUrls = null;
                if (request.Images != null && request.Images.Any())
                {                   
                    var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var invalidFiles = request.Images.Where(file =>
                        !validImageExtensions.Any(ext =>
                            file.FileName.ToLower().EndsWith(ext))).ToList();

                    if (invalidFiles.Any())
                    {
                        return (CreateCommentEnum.InvalidImageFormat, null);
                    }
                  
                    const long maxFileSize = 10 * 1024 * 1024;
                    var oversizedFiles = request.Images.Where(file => file.Length > maxFileSize).ToList();

                    if (oversizedFiles.Any())
                    {
                        return (CreateCommentEnum.FileTooLarge, null);
                    }
                  
                    imageUrls = await _uploadService.UploadFile(request.Images, "comments/images");
                    if (imageUrls == null || !imageUrls.Any())
                    {
                        return (CreateCommentEnum.ImageUploadFailed, null);
                    }
                }
          
                var comment = new Comment
                {
                    Content = request.Content.Trim(),
                    //CreatedAt = DateTime.UtcNow,
                    //UpdatedAt = DateTime.UtcNow,
                    //TotalLiked = 0,
                    PostId = request.PostId,
                    UserId = userId,
                    RepliedCommentId = request.RepliedCommentId
                };
                
                if (imageUrls != null && imageUrls.Any())
                {
                    comment.CommentImage = imageUrls
                        .Where(url => !string.IsNullOrWhiteSpace(url))
                        .Select(imageUrl => new CommentImage
                        {
                            ImageUrl = imageUrl
                        }).ToList();
                }

                _unitOfWork.CommentRepository.Add(comment);
            
                post.TotalComment += 1;
                _unitOfWork.PostRepository.Update(post);

                var result = await _unitOfWork.CompleteAsync();
                if (result > 0)
                {
                    return (CreateCommentEnum.CreateCommentSuccess, comment.Id);
                }

                return (CreateCommentEnum.CreateCommentFailed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when creating comment for post {PostId} by user {UserId}", request.PostId, userId);
                return (CreateCommentEnum.CreateCommentFailed, null);
            }
        }

        public async Task<(GetCommentsEnum, List<CommentDto>?)> GetCommentsByPostIdAsync(Guid postId, int skip = 0, int take = 10)
        {
            try
            {
                var post = await _unitOfWork.PostRepository.GetByIdAsync(postId);
                if (post == null)
                {
                    return (GetCommentsEnum.PostNotFound, null);
                }

                var comments = await _unitOfWork.CommentRepository.GetCommentsByPostIdAsync(postId, skip, take);

                if (comments == null || !comments.Any())
                {
                    return (GetCommentsEnum.NoCommentsFound, null);
                }

                // Tạo HashSet để theo dõi các comment đã xử lý
                var processedComments = new HashSet<Guid>();

                // Sử dụng phương thức đệ quy để ánh xạ các comment và replies
                var commentDtos = comments.Select(comment => MapCommentToDto(comment, processedComments)).Where(dto => dto != null).ToList();

                return (GetCommentsEnum.Success, commentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting comments for post {PostId}", postId);
                return (GetCommentsEnum.Failed, null);
            }
        }

        // Đệ quy để ánh xạ Comment sang CommentDto
        private CommentDto MapCommentToDto(Comment comment, HashSet<Guid> processedComments)
        {
            // Nếu comment đã được xử lý, trả về null để tránh vòng lặp
            if (processedComments.Contains(comment.Id))
            {
                return null;
            }

            // Đánh dấu comment này là đã xử lý
            processedComments.Add(comment.Id);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                UserId = comment.UserId,
                RepliedCommentId = comment.RepliedCommentId,
                User = comment.User == null ? null : new UserDto
                {
                    Id = comment.User.Id,
                    Email = comment.User.Email,
                    UserName = comment.User.UserName ?? "",
                    Status = comment.User.Status.ToString(),
                    FirstName = comment.User.FirstName,
                    LastName = comment.User.LastName,
                    AvatarUrl = comment.User.AvatarUrl
                },
                CommentImages = comment.CommentImage?.Select(img => new CommentImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl
                }).ToList(),
                Replies = comment.Replies?.Select(reply => MapCommentToDto(reply, processedComments)).Where(r => r != null).ToList(),
                CommentReactionUsers = comment.CommentReactionUsers
            };
        }

        public async Task<(UpdateCommentEnum, CommentDto?)> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request, Guid userId)
        {
            try
            {
                var comments = await _unitOfWork.CommentRepository.FindAsyncWithIncludes(
                    c => c.Id == commentId,
                    c => c.User,
                    c => c.CommentImage,
                    c => c.CommentReactionUsers
                );

                var comment = comments?.FirstOrDefault();
                if (comment == null)
                {
                    return (UpdateCommentEnum.CommentNotFound, null);
                }

                if (comment.UserId != userId)
                {
                    return (UpdateCommentEnum.Unauthorized, null);
                }
                
                if (request.Content != null)
                {
                    if (string.IsNullOrWhiteSpace(request.Content))
                    {
                        return (UpdateCommentEnum.InvalidContent, null);
                    }
                    comment.Content = request.Content.Trim();
                }
               
                else if (request.ImageIdsToDelete != null && request.ImageIdsToDelete.Any())
                {
                    var imagesToDelete = comment.CommentImage?
                        .Where(img => request.ImageIdsToDelete.Contains(img.Id))
                        .ToList();

                    if (imagesToDelete != null)
                    {
                        foreach (var image in imagesToDelete)
                        {
                            comment.CommentImage.Remove(image);
                        }
                    }
                }
               
                if (request.NewImages != null && request.NewImages.Any())
                {
                    // Validate file types
                    var validImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var invalidFiles = request.NewImages.Where(file =>
                        !validImageExtensions.Any(ext =>
                            file.FileName.ToLower().EndsWith(ext))).ToList();

                    if (invalidFiles.Any())
                    {
                        return (UpdateCommentEnum.InvalidImageFormat, null);
                    }
                   
                    const long maxFileSize = 10 * 1024 * 1024;
                    var oversizedFiles = request.NewImages.Where(file => file.Length > maxFileSize).ToList();

                    if (oversizedFiles.Any())
                    {
                        return (UpdateCommentEnum.FileTooLarge, null);
                    }
                 
                    var newImageUrls = await _uploadService.UploadFile(request.NewImages, "comments/images");
                    if (newImageUrls == null || !newImageUrls.Any())
                    {
                        return (UpdateCommentEnum.ImageUploadFailed, null);
                    }

                    var newCommentImages = newImageUrls
                        .Where(url => !string.IsNullOrWhiteSpace(url))
                        .Select(imageUrl => new CommentImage
                        {
                            CommentId = comment.Id,
                            ImageUrl = imageUrl
                        }).ToList();

                    if (comment.CommentImage == null)
                    {
                        comment.CommentImage = new List<CommentImage>();
                    }

                    foreach (var newImage in newCommentImages)
                    {
                        comment.CommentImage.Add(newImage);
                    }
                }

                //comment.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.CommentRepository.Update(comment);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    var commentDto = new CommentDto
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        //CreatedAt = comment.CreatedAt,
                        //UpdatedAt = comment.UpdatedAt,
                        //TotalLiked = comment.TotalLiked,
                        PostId = comment.PostId,
                        UserId = comment.UserId,
                        RepliedCommentId = comment.RepliedCommentId,
                        User = comment.User == null ? null : new UserDto
                        {
                            Id = comment.User.Id,
                            Email = comment.User.Email,
                            UserName = comment.User.UserName ?? "",
                            Status = comment.User.Status.ToString(),
                            FirstName = comment.User.FirstName,
                            LastName = comment.User.LastName,
                            AvatarUrl = comment.User.AvatarUrl
                        },
                        CommentImages = comment.CommentImage?.Select(img => new CommentImageDto
                        {
                            Id = img.Id,
                            ImageUrl = img.ImageUrl
                        }).ToList(),
                        CommentReactionUsers = comment.CommentReactionUsers
                    };

                    return (UpdateCommentEnum.UpdateCommentSuccess, commentDto);
                }

                return (UpdateCommentEnum.UpdateCommentFailed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating comment {CommentId} by user {UserId}", commentId, userId);
                return (UpdateCommentEnum.UpdateCommentFailed, null);
            }
        }

        public async Task<(DeleteCommentEnum, bool)> DeleteCommentAsync(Guid commentId, Guid userId)
        {
            try
            {
                var comments = await _unitOfWork.CommentRepository.FindAsyncWithIncludes(
                    c => c.Id == commentId,
                    c => c.Post
                );

                var comment = comments?.FirstOrDefault();
                if (comment == null)
                {
                    return (DeleteCommentEnum.CommentNotFound, false);
                }

                if (comment.UserId != userId)
                {
                    return (DeleteCommentEnum.Unauthorized, false);
                }

                _unitOfWork.CommentRepository.Remove(comment);

                // Cập nhật tổng số comment của post
                if (comment.Post != null)
                {
                    comment.Post.TotalComment = Math.Max(0, comment.Post.TotalComment - 1);
                    _unitOfWork.PostRepository.Update(comment.Post);
                }

                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    return (DeleteCommentEnum.DeleteCommentSuccess, true);
                }

                return (DeleteCommentEnum.DeleteCommentFailed, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting comment {CommentId} by user {UserId}", commentId, userId);
                return (DeleteCommentEnum.DeleteCommentFailed, false);
            }
        }

        public async Task<CommentDto?> AddUpdateDeleteReactionComment(ReactionCommentRequest request, Guid userId)
        {
            try
            {              
                var commentReactionUser = await _unitOfWork.CommentReactionUserRepository
                    .FindFirstAsync(r => r.CommentId == request.CommentId && r.UserId == userId);

                var comments = await _unitOfWork.CommentRepository.FindAsyncWithIncludes(
                    c => c.Id == request.CommentId,
                    c => c.User,
                    c => c.CommentImage,
                    c => c.CommentReactionUsers
                );

                var comment = comments?.FirstOrDefault();
                if (comment == null) return null;

                if (commentReactionUser == null)
                {                    
                    commentReactionUser = new CommentReactionUser
                    {
                        UserId = userId,
                        Reaction = request.Reaction,
                        CommentId = request.CommentId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    };

                    _unitOfWork.CommentReactionUserRepository.Add(commentReactionUser);
                    //comment.TotalLiked += 1;
                }
                else if (commentReactionUser.Reaction == request.Reaction)
                {
                    // Xóa reaction nếu trùng
                    _unitOfWork.CommentReactionUserRepository.Remove(commentReactionUser);
                    //comment.TotalLiked = Math.Max(0, comment.TotalLiked - 1);
                }
                else
                {
                    // Cập nhật reaction khác
                    commentReactionUser.Reaction = request.Reaction;
                    commentReactionUser.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.CommentReactionUserRepository.Update(commentReactionUser);
                }

                _unitOfWork.CommentRepository.Update(comment);
                await _unitOfWork.CompleteAsync();

                var commentDto = new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    //CreatedAt = comment.CreatedAt,
                    //UpdatedAt = comment.UpdatedAt,
                    //TotalLiked = comment.TotalLiked,
                    PostId = comment.PostId,
                    UserId = comment.UserId,
                    RepliedCommentId = comment.RepliedCommentId,
                    User = comment.User == null ? null : new UserDto
                    {
                        Id = comment.User.Id,
                        Email = comment.User.Email,
                        UserName = comment.User.UserName ?? "",
                        Status = comment.User.Status.ToString(),
                        FirstName = comment.User.FirstName,
                        LastName = comment.User.LastName,
                        AvatarUrl = comment.User.AvatarUrl
                    },
                    CommentImages = comment.CommentImage?.Select(img => new CommentImageDto
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl
                    }).ToList(),
                    CommentReactionUsers = comment.CommentReactionUsers
                };

                return commentDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reacting to comment {CommentId}", request.CommentId);
                return null;
            }
        }
    }
}