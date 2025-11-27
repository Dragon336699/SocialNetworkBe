using Domain.Contracts.Responses.Notification;
using Domain.Enum.Notification.Types;
using Domain.Entities;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using Microsoft.AspNetCore.SignalR;
using SocialNetworkBe.ChatServer;
using Domain.Contracts.Responses.User;
using System.Text;
using System.Runtime.Intrinsics.X86;
using Domain.Contracts.Responses.Post;
using Domain.Enum.Post.Functions;
using Microsoft.Extensions.Hosting;
using SocialNetworkBe.Services.PostServices;

namespace SocialNetworkBe.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;
        private readonly IRealtimeService _realtimeService;
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPostReactionUserService _postReactionUserService;
        public NotificationService(
            IUnitOfWork unitOfWork,
            ILogger<NotificationService> logger,
            IRealtimeService realtimeService,
            IUserService userService,
            ICommentService commentService,
            IServiceProvider serviceProvider,
            IPostReactionUserService postReactionUserService
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _realtimeService = realtimeService;
            _userService = userService;
            _commentService = commentService;
            _serviceProvider = serviceProvider;
            _postReactionUserService = postReactionUserService;
        }

        public async Task ProcessAndSendNotiForReactPost(NotificationType type, NotificationData? data, string navigateUrl, string mergeKey, Guid receiverId)
        {
            try
            {
                Notification? noti = await _unitOfWork.NotificationRepository.FindFirstAsync(n => n.MergeKey == mergeKey);
                if (data == null) return;

                bool notiNull = noti == null;
                if (noti == null)
                {
                    Notification newNoti = new Notification
                    {
                        NoficationType = type,
                        Data = data,
                        MergeKey = mergeKey,
                        NavigateUrl = navigateUrl,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        ReceiverId = receiverId
                    };
                    noti = newNoti;
                }
                else
                {
                    
                    if (noti.Data.Subjects.Any(s => s.Id == data.Subjects[0].Id))
                    {
                        
                    } else
                    {
                        noti.Data.SubjectCount += data.SubjectCount;
                        noti.Data.Subjects.AddRange(data.Subjects);
                        while (noti.Data.Subjects.Count > 2)
                        {
                            noti.Data.Subjects.RemoveAt(0);
                        }
                    }
                }

                NotificationDto notiDto = await CreateNotificationDto(noti, receiverId);
                notiDto.Unread = true;

                await _realtimeService.SendPrivateNotification(notiDto, receiverId);
                if (notiNull) _unitOfWork.NotificationRepository.Add(noti);
                else _unitOfWork.NotificationRepository.Update(noti);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while sending notification react post");
                throw;
            }
        }

        public async Task ProcessAndSendNotiForCommentPost(NotificationType type, NotificationData data, string navigateUrl, string mergeKey, Guid receiverId)
        {
            try
            {
                Notification newNoti = new Notification
                {
                    NoficationType = type,
                    Data = data,
                    MergeKey = mergeKey,
                    NavigateUrl = navigateUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ReceiverId = receiverId
                };
                NotificationDto notiDto = await CreateNotificationDto(newNoti, receiverId);
                notiDto.Unread = true;

                await _realtimeService.SendPrivateNotification(notiDto, receiverId);
                _unitOfWork.NotificationRepository.Add(newNoti);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while sending notification comment post");
                throw;
            }
        }

        public async Task<List<NotificationDto>?> GetNotis(Guid userId, int skip, int take)
        {

            try
            {
                List<Notification> notis = await _unitOfWork.NotificationRepository.GetNotis(userId, skip, take);
                List<NotificationDto> notiDtos = new List<NotificationDto>();

                foreach (var noti in notis)
                {
                    NotificationDto notiDto = await CreateNotificationDto(noti, userId);
                    notiDtos.Add(notiDto);
                }

                if (notis == null) return null;
                return notiDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting notifications");
                throw;
            }
        }

        public async Task<int> GetUnreadNotifications(Guid userId)
        {
            try
            {
                int unreadNotis = await _unitOfWork.NotificationRepository.GetUnreadNotifications(userId);
                return unreadNotis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting notifications unread");
                throw;
            }
        }

        public async Task MarkNotificationAsRead(Guid notificationId)
        {
            try
            {
                Notification? noti = await _unitOfWork.NotificationRepository.FindFirstAsync(n => n.Id == notificationId);
                if (noti == null) return;
                noti.Unread = false;
                _unitOfWork.NotificationRepository.Update(noti);
                _unitOfWork.Complete();
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while mark notification read");
                throw;
            }
        }

        public async Task MarkAllNotificationsAsRead()
        {
            try
            {
                await _unitOfWork.NotificationRepository.MarkAllNotificationsAsRead();
                _unitOfWork.Complete();
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while mark notification read");
                throw;
            }
        }

        private async Task<NotificationDto> CreateNotificationDto(Notification noti, Guid userId)
        {
            var postService = _serviceProvider.GetRequiredService<IPostService>();
            var commentService = _serviceProvider.GetRequiredService<ICommentService>();

            List<HighlightOffset> highlightOffsets = new List<HighlightOffset>();
            StringBuilder content = new StringBuilder();
            List<string> imageUrls = new List<string>();
            UserDto? user1 = await _userService.GetUserInfoByUserId(noti.Data.Subjects[0].Id.ToString());
            imageUrls.Add(user1.AvatarUrl);
            string prObjectName = "";
            string diObjectName = "";

            HighlightOffset offset = new HighlightOffset
            {
                Offset = 0,
                Length = (user1.LastName + " " + user1.FirstName).Trim().Length,
            };
            highlightOffsets.Add(offset);
            if (noti.Data.SubjectCount > 1)
            {
                UserDto? user2 = await _userService.GetUserInfoByUserId(noti.Data.Subjects[1].Id.ToString());
                imageUrls.Add(user2.AvatarUrl);
                var name1 = $"{user1.LastName} {user1.FirstName}".Trim();
                var name2 = $"{user2.LastName} {user2.FirstName}".Trim();
                content.Append($"{name1}, {name2}");

                offset = new HighlightOffset
                {
                    Offset = offset.Length + 2,
                    Length = (user2.LastName + " " + user2.FirstName).Trim().Length
                };

                highlightOffsets.Add(offset);

                if (noti.Data.SubjectCount > 2)
                {
                    content.Append($" and {noti.Data.SubjectCount} {(noti.Data.SubjectCount == 3 ? "other" : "others")} ");
                }
            }
            else
            {
                content.Append($"{user1.LastName} {user1.FirstName} ");
            }

            content.Append($"{noti.Data.Verb.ToString().ToLower()} {noti.Data.InObject?.Type.ToString().ToLower()} ");
            switch (noti.Data.DiObject.Type)
            {
                case (NotificationObjectType.Post):
                    {
                        var (state, post) = await postService.GetPostByIdAsync(noti.Data.DiObject.Id.Value, userId);
                        diObjectName = post.Content.Length > 30 ? post.Content.Substring(0, 30) + "..." : post.Content;
                        content.Append($"your {noti.Data.DiObject.Type.ToString().ToLower()} \"{diObjectName}\" ");
                        break;
                    }
                case (NotificationObjectType.Comment):
                    {
                        var (state, comment) = await commentService.GetCommentById(noti.Data.DiObject.Id.Value);
                        diObjectName = comment.Content.Length > 30 ? comment.Content.Substring(0, 30) + "..." : comment.Content;
                        content.Append($"\"{diObjectName}\" ");
                        break;
                    }
            }
            if (noti.Data.Preposition != null)
            {
                content.Append($"{noti.Data.Preposition?.ToString().ToLower()} ");
            }

            if (noti.Data.PrObject != null && noti.Data.PrObject.Id != null)
            {
                switch (noti.Data?.PrObject?.Type)
                {
                    case NotificationObjectType.Post:
                        {
                            var (state, post) = await postService.GetPostByIdAsync(noti.Data.PrObject.Id.Value, userId);
                            prObjectName = post.Content.Length > 30 ? post.Content.Substring(0, 30) + "..." : post.Content;
                            break;
                        }

                    case NotificationObjectType.Comment:
                        {
                            var (state, comment) = await _commentService.GetCommentById(noti.Data.PrObject.Id.Value);
                            prObjectName = comment.Content.Length > 30 ? comment.Content.Substring(0, 30) + "..." : comment.Content;
                            break;
                        }

                }
                content.Append($"{noti.Data.PrObject.Type.ToString().ToLower()} '{prObjectName}'");
            }
            NotificationDto notiDto = new NotificationDto
            {
                Id = noti.Id,
                Content = content.ToString().Trim(),
                ImageUrls = imageUrls,
                Unread = noti.Unread,
                CreatedAt = noti.CreatedAt.ToLocalTime(),
                UpdatedAt = noti.UpdatedAt.ToLocalTime(),
                ReceiverId = noti.ReceiverId,
                Highlights = highlightOffsets
            };
            return notiDto;
        }
    }
}
