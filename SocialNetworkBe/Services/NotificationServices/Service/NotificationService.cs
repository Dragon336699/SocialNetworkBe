using Domain.Contracts.Responses.Notification;
using Domain.Enum.Notification.Types;
using Domain.Entities;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using Microsoft.AspNetCore.SignalR;
using SocialNetworkBe.ChatServer;

namespace SocialNetworkBe.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;
        private readonly IRealtimeService _realtimeService;
        public NotificationService(
            IUnitOfWork unitOfWork,
            ILogger<NotificationService> logger,
            IRealtimeService realtimeService
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _realtimeService = realtimeService;
        }

        public async Task ProcessAndSendNotiForReactPost(NotificationType type, NotificationData data, string navigateUrl, string mergeKey, Guid receiverId)
        {
            try
            {
                Notification? noti = await _unitOfWork.NotificationRepository.FindFirstAsync(n => n.MergeKey == mergeKey);
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
                } else
                {
                    noti.Data.SubjectCount += data.SubjectCount;
                    noti.Data.Subjects.AddRange(data.Subjects);
                    while (noti.Data.Subjects.Count > 2)
                    {
                        noti.Data.Subjects.RemoveAt(0);
                    }
                }

                List<HighlightOffset> highlights = new List<HighlightOffset>();

                if (noti.Data.SubjectCount == 1)
                {
                    noti.Data.Content = noti.Data.Subjects[0].Name.Trim() + " reacted your post";
                    HighlightOffset highlight = new HighlightOffset
                    {
                        Offset = 0,
                        Length = noti.Data.Subjects[0].Name.Trim().Length
                    };
                    highlights.Add(highlight);
                } else if (noti.Data.SubjectCount == 2)
                {
                    noti.Data.Content = noti.Data.Subjects[0].Name.Trim() + " and " + noti.Data.Subjects[1].Name.Trim() + " reacted your post";
                    int offset = 0;
                    foreach (var subject in noti.Data.Subjects)
                    {
                        HighlightOffset highlight = new HighlightOffset
                        {
                            Offset = offset,
                            Length = subject.Name.Trim().Length
                        };
                        highlights.Add(highlight);
                        offset += subject.Name.Trim().Length + 5;
                    }

                } else if (noti.Data.SubjectCount > 2)
                {
                    noti.Data.Content = $"{noti.Data.Subjects[0].Name.Trim()}, {noti.Data.Subjects[1].Name.Trim()} and " +
                        $"{(noti.Data.SubjectCount - 2)} " +
                        $"{(noti.Data.SubjectCount - 2 > 1 ? "others" : "other")} reacted your post";
                    int offset = 0;
                    foreach (var subject in noti.Data.Subjects)
                    {
                        HighlightOffset highlight = new HighlightOffset
                        {
                            Offset = offset,
                            Length = subject.Name.Trim().Length
                        };
                        highlights.Add(highlight);
                        offset += subject.Name.Trim().Length + 2;
                    }
                }

                noti.Data.Highlights = highlights;
                await _realtimeService.SendPrivateNotification(noti, receiverId);
                if (notiNull) _unitOfWork.NotificationRepository.Add(noti);
                else _unitOfWork.NotificationRepository.Update(noti);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while sending notification");
                throw;
            }
        }

        public async Task<List<Notification>?> GetNotis (Guid userId, int skip, int take)
        {
            try
            {
                List<Notification> notis = await _unitOfWork.NotificationRepository.GetNotis(userId, skip, take);
                if (notis == null) return null;
                return notis;
            } catch (Exception ex)
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
    }
}
