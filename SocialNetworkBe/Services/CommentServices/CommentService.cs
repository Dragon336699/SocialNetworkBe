using AutoMapper;
using Domain.Contracts.Requests.Comment;
using Domain.Contracts.Responses.Comment;
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
        private readonly IMapper _mapper;

        public CommentService(
            IUnitOfWork unitOfWork,
            ILogger<CommentService> logger,
            IUploadService uploadService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _uploadService = uploadService;
            _mapper = mapper;
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

                var currentTime = DateTime.UtcNow;
                var comment = new Comment
                {
                    Content = request.Content.Trim(),
                    PostId = request.PostId,
                    UserId = userId,
                    RepliedCommentId = request.RepliedCommentId,
                    CreatedAt = currentTime,
                    UpdatedAt = currentTime,
                    TotalLiked = 0
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

                var commentDtos = _mapper.Map<List<CommentDto>>(comments);

                return (GetCommentsEnum.Success, commentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting comments for post {PostId}", postId);
                return (GetCommentsEnum.Failed, null);
            }
        }

        public async Task<(UpdateCommentEnum, CommentDto?)> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request, Guid userId)
        {
            try
            {             
                var comment = await _unitOfWork.CommentRepository.GetCommentByIdWithTrackingAsync(commentId);

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

                if (request.ImageIdsToDelete != null && request.ImageIdsToDelete.Any())
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
                comment.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.CommentRepository.Update(comment);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {                   
                    var commentDto = _mapper.Map<CommentDto>(comment);
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
                var comment = await _unitOfWork.CommentRepository.GetCommentByIdWithTrackingAsync(commentId);

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

                var comment = await _unitOfWork.CommentRepository.GetCommentByIdWithTrackingAsync(request.CommentId);

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
                    comment.TotalLiked += 1;
                }
                else if (commentReactionUser.Reaction == request.Reaction)
                {
                    // Xóa reaction nếu trùng
                    _unitOfWork.CommentReactionUserRepository.Remove(commentReactionUser);
                    comment.TotalLiked = Math.Max(0, comment.TotalLiked - 1);
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
               
                var commentDto = _mapper.Map<CommentDto>(comment);
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