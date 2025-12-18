using Domain.Contracts.Requests.FriendRequest;
using Domain.Contracts.Responses.FriendRequest;
using Domain.Contracts.Responses.Notification;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.FriendRequest.Functions;
using Domain.Enum.Notification.Types;
using Domain.Enum.User.Types;
using Domain.Interfaces.BuilderInterfaces;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using SocialNetworkBe.Services.NotificationService;

namespace SocialNetworkBe.Services.FriendRequestServices
{
    public class FriendRequestService : IFriendRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FriendRequestService> _logger;
        private readonly INotificationDataBuilder _notificationDataBuilder;
        private readonly IServiceProvider _serviceProvider;

        public FriendRequestService(IUnitOfWork unitOfWork, ILogger<FriendRequestService> logger, INotificationDataBuilder notificationDataBuilder, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _notificationDataBuilder = notificationDataBuilder;
            _serviceProvider = serviceProvider;
        }

        public async Task<(SendFriendRequestEnum, FriendRequestDto?)> SendFriendRequestAsync(SendFriendRequestRequest request, Guid senderId)
        {
            try
            {
                var notificationService = _serviceProvider.GetRequiredService<INotificationService>();
                var sender = await _unitOfWork.UserRepository.GetByIdAsync(senderId);
                if (sender == null)
                {
                    return (SendFriendRequestEnum.SenderNotFound, null);
                }

                var receiver = await _unitOfWork.UserRepository.GetByIdAsync(request.ReceiverId);
                if (receiver == null)
                {
                    return (SendFriendRequestEnum.ReceiverNotFound, null);
                }

                if (senderId == request.ReceiverId)
                {
                    return (SendFriendRequestEnum.CannotSendToSelf, null);
                }

                // Kiểm tra xem hai người đã là bạn bè chưa
                if (await _unitOfWork.FriendRequestRepository.AreFriendsAsync(senderId, request.ReceiverId))
                {
                    return (SendFriendRequestEnum.AlreadyFriends, null);
                }

                // Kiểm tra xem lời mời đã tồn tại chưa
                var existingRequest = await _unitOfWork.FriendRequestRepository.GetFriendRequestAsync(senderId, request.ReceiverId);
                if (existingRequest != null)
                {
                    return (SendFriendRequestEnum.RequestAlreadyExists, null);
                }

                // Kiểm tra xem người nhận có chặn người gửi không
                var userRelations = await _unitOfWork.UserRelationRepository.FindAsync(
                    ur => ur.UserId == request.ReceiverId && ur.RelatedUserId == senderId && ur.RelationType == UserRelationType.Blocked);

                if (userRelations != null && userRelations.Any())
                {
                    return (SendFriendRequestEnum.ReceiverBlocked, null);
                }

                var friendRequest = new FriendRequest
                {
                    SenderId = senderId,
                    ReceiverId = request.ReceiverId,
                    CreatedAt = DateTime.UtcNow,
                    FriendRequestStatus = FriendRequestStatus.Pending.ToString(),
                };

                _unitOfWork.FriendRequestRepository.Add(friendRequest);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    // Send notification
                    NotificationData? notiData = _notificationDataBuilder.BuilderDataForFriendRequest(sender);
                    string navigateUrl = $"/profile/{sender.UserName}";
                    await notificationService.ProcessAndSendNotiForFriendRequest(NotificationType.AddFriendRequest, notiData, navigateUrl, request.ReceiverId);

                    var friendRequestDto = new FriendRequestDto
                    {
                        SenderId = friendRequest.SenderId,
                        ReceiverId = friendRequest.ReceiverId,
                        Status = Enum.Parse<FriendRequestStatus>(friendRequest.FriendRequestStatus),
                        Sender = new UserDto
                        {
                            Id = sender.Id,
                            Email = sender.Email,
                            UserName = sender.UserName ?? "",
                            Status = sender.Status.ToString(),
                            FirstName = sender.FirstName,
                            LastName = sender.LastName,
                            AvatarUrl = sender.AvatarUrl
                        },
                        Receiver = new UserDto
                        {
                            Id = receiver.Id,
                            Email = receiver.Email,
                            UserName = receiver.UserName ?? "",
                            Status = receiver.Status.ToString(),
                            FirstName = receiver.FirstName,
                            LastName = receiver.LastName,
                            AvatarUrl = receiver.AvatarUrl
                        }
                    };

                    return (SendFriendRequestEnum.SendFriendRequestSuccess, friendRequestDto);
                }

                return (SendFriendRequestEnum.SendFriendRequestFailed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when sending friend request from {SenderId} to {ReceiverId}", senderId, request.ReceiverId);
                return (SendFriendRequestEnum.SendFriendRequestFailed, null);
            }
        }

        public async Task<(RespondFriendRequestEnum, FriendRequestDto?)> ApproveFriendRequestAsync(RespondFriendRequestRequest request, Guid receiverId)
        {
            try
            {
                var notificationService = _serviceProvider.GetRequiredService<INotificationService>();
                // Lấy thông tin lời mời kết bạn
                var friendRequest = await _unitOfWork.FriendRequestRepository.GetFriendRequestAsync(request.SenderId, receiverId);
                var receiver = await _unitOfWork.UserRepository.GetByIdAsync(receiverId);
                if (receiver == null)
                {
                    return (RespondFriendRequestEnum.ReceiverNotFound, null);
                }

                if (friendRequest == null)
                {
                    return (RespondFriendRequestEnum.FriendRequestNotFound, null);
                }

                _unitOfWork.FriendRequestRepository.Remove(friendRequest);

                var userRelation = new UserRelation
                {
                    UserId = friendRequest.SenderId,
                    RelatedUserId = friendRequest.ReceiverId,
                    RelationType = UserRelationType.Friend,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _unitOfWork.UserRelationRepository.Add(userRelation);

                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    // Send notification
                    NotificationData? notiData = _notificationDataBuilder.BuilderDataForAcceptFriendRequest(receiver);
                    string navigateUrl = $"/profile/{receiver.UserName}";
                    await notificationService.ProcessAndSendNotiForFriendRequest(NotificationType.AcceptFriendRequest, notiData, navigateUrl, request.SenderId);

                    var friendRequestDto = new FriendRequestDto
                    {
                        SenderId = friendRequest.SenderId,
                        ReceiverId = friendRequest.ReceiverId,
                        Status = Enum.Parse<FriendRequestStatus>(friendRequest.FriendRequestStatus),
                        Sender = friendRequest.Sender == null ? null : new UserDto
                        {
                            Id = friendRequest.Sender.Id,
                            Email = friendRequest.Sender.Email,
                            UserName = friendRequest.Sender.UserName ?? "",
                            Status = friendRequest.Sender.Status.ToString(),
                            FirstName = friendRequest.Sender.FirstName,
                            LastName = friendRequest.Sender.LastName,
                            AvatarUrl = friendRequest.Sender.AvatarUrl
                        },
                        Receiver = friendRequest.Receiver == null ? null : new UserDto
                        {
                            Id = friendRequest.Receiver.Id,
                            Email = friendRequest.Receiver.Email,
                            UserName = friendRequest.Receiver.UserName ?? "",
                            Status = friendRequest.Receiver.Status.ToString(),
                            FirstName = friendRequest.Receiver.FirstName,
                            LastName = friendRequest.Receiver.LastName,
                            AvatarUrl = friendRequest.Receiver.AvatarUrl
                        }
                    };

                    return (RespondFriendRequestEnum.RespondFriendRequestSuccess, friendRequestDto);
                }

                return (RespondFriendRequestEnum.RespondFriendRequestFailed, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when responding to friend request from {SenderId} to {ReceiverId}", request.SenderId, receiverId);
                return (RespondFriendRequestEnum.RespondFriendRequestFailed, null);
            }
        }

        public async Task<RespondFriendRequestEnum> DeclineFriendRequestAsync(RespondFriendRequestRequest request, Guid receiverId)
        {
            try
            {
                // Lấy thông tin lời mời kết bạn
                var friendRequest = await _unitOfWork.FriendRequestRepository.GetFriendRequestAsync(request.SenderId, receiverId);
                if (friendRequest == null)
                {
                    return (RespondFriendRequestEnum.FriendRequestNotFound);
                }

                _unitOfWork.FriendRequestRepository.Remove(friendRequest);

                await _unitOfWork.CompleteAsync();

                return (RespondFriendRequestEnum.RespondFriendRequestSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when responding to friend request from {SenderId} to {ReceiverId}", request.SenderId, receiverId);
                return (RespondFriendRequestEnum.RespondFriendRequestFailed);
            }
        }
        public async Task<List<FriendRequestDto>> GetSentFriendRequestsAsync(Guid senderId, int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            var (friendRequests, totalCount) = await _unitOfWork.FriendRequestRepository.GetSentFriendRequestsAsync(senderId, pageIndex, pageSize);

            var requestDtos = friendRequests.Select(fr => new FriendRequestDto
            {
                SenderId = fr.SenderId,
                CreatedAt = fr.CreatedAt,
                ReceiverId = fr.ReceiverId,
                Status = Enum.Parse<FriendRequestStatus>(fr.FriendRequestStatus),
                Receiver = fr.Receiver == null ? null : new UserDto
                {
                    Id = fr.Receiver.Id,
                    Email = fr.Receiver.Email,
                    UserName = fr.Receiver.UserName ?? "",
                    Status = fr.Receiver.Status.ToString(),
                    FirstName = fr.Receiver.FirstName,
                    LastName = fr.Receiver.LastName,
                    AvatarUrl = fr.Receiver.AvatarUrl
                }
            }).ToList();

            return requestDtos;
        }

        public async Task<CancelFriendRequestEnum> CancelFriendRequestAsync(CancelFriendRequestRequest request, Guid senderId)
        {
            try
            {
                var friendRequest = await _unitOfWork.FriendRequestRepository.GetFriendRequestAsync(senderId, request.ReceiverId);

                if (friendRequest == null)
                {
                    return CancelFriendRequestEnum.RequestNotFound;
                }

                if (friendRequest.FriendRequestStatus != FriendRequestStatus.Pending.ToString())
                {
                    return CancelFriendRequestEnum.NotPending;
                }

                _unitOfWork.FriendRequestRepository.Remove(friendRequest);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    return CancelFriendRequestEnum.Success;
                }

                return CancelFriendRequestEnum.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when cancelling friend request from {SenderId} to {ReceiverId}", senderId, request.ReceiverId);
                return CancelFriendRequestEnum.Failed;
            }
        }
        public async Task<List<FriendRequestDto>> GetReceivedFriendRequestsAsync(Guid receiverId, int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            var (friendRequests, totalCount) = await _unitOfWork.FriendRequestRepository.GetReceivedFriendRequestsAsync(receiverId, pageIndex, pageSize);

            var requestDtos = friendRequests.Select(fr => new FriendRequestDto
            {
                SenderId = fr.SenderId,
                ReceiverId = fr.ReceiverId,
                Status = Enum.Parse<FriendRequestStatus>(fr.FriendRequestStatus),
                CreatedAt = fr.CreatedAt,

                Sender = fr.Sender == null ? null : new UserDto
                {
                    Id = fr.Sender.Id,
                    Email = fr.Sender.Email,
                    UserName = fr.Sender.UserName ?? "",
                    Status = fr.Sender.Status.ToString(),
                    FirstName = fr.Sender.FirstName,
                    LastName = fr.Sender.LastName,
                    AvatarUrl = fr.Sender.AvatarUrl
                },
                Receiver = null
            }).ToList();

            return requestDtos;
        }

    }
}