using Domain.Contracts.Requests.FriendRequest;
using Domain.Contracts.Responses.FriendRequest;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.FriendRequest.Functions;
using Domain.Enum.User.Types;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace SocialNetworkBe.Services.FriendRequestServices
{
    public class FriendRequestService : IFriendRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FriendRequestService> _logger;

        public FriendRequestService(IUnitOfWork unitOfWork, ILogger<FriendRequestService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<(SendFriendRequestEnum, FriendRequestDto?)> SendFriendRequestAsync(SendFriendRequestRequest request, Guid senderId)
        {
            try
            {
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
                    FriendRequestStatus = FriendRequestStatus.Pending.ToString(),              
                };

                _unitOfWork.FriendRequestRepository.Add(friendRequest);
                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
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
        public async Task<bool> CancelFriendRequestAsync(Guid senderId, SendFriendRequestRequest req)
        {
            var request = await _unitOfWork.FriendRequestRepository.GetFriendRequestAsync(senderId, req.ReceiverId);

            if (request == null)
                return false;

            return await _unitOfWork.FriendRequestRepository.DeleteFriendRequestAsync(request);
        }

        public async Task<(GetFriendRequestsEnum, List<FriendRequestDto>?)> GetSentFriendRequestsAsync(Guid senderId, int skip = 0, int take = 10)
        {
            try
            {
                var sender = await _unitOfWork.UserRepository.GetByIdAsync(senderId);
                if (sender == null)
                {
                    _logger.LogWarning("Sender not found when getting sent friend requests. SenderId: {SenderId}", senderId);
                    return (GetFriendRequestsEnum.SenderNotFound, null);
                }

                var sentRequests = await _unitOfWork.FriendRequestRepository.GetSentFriendRequestsAsync(senderId);
                if (sentRequests == null || !sentRequests.Any())
                {
                    return (GetFriendRequestsEnum.NoRequestsFound, null);
                }

                IEnumerable<FriendRequest> sortedRequests = sentRequests;

                if (skip > 0 || take != 10) // nghĩa là có truyền phân trang
                {
                    sortedRequests = sentRequests
                        .Skip(skip)
                        .Take(take);
                }

                var requestDtos = sortedRequests.Select(fr => new FriendRequestDto
                {
                    SenderId = fr.SenderId,
                    ReceiverId = fr.ReceiverId,
                    Status = Enum.Parse<FriendRequestStatus>(fr.FriendRequestStatus),
                    Receiver = new UserDto
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

                return (GetFriendRequestsEnum.Success, requestDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting sent friend requests for {SenderId}", senderId);
                return (GetFriendRequestsEnum.Failed, null);
            }
        }

        public async Task<(RespondFriendRequestEnum, FriendRequestDto?)> RespondFriendRequestAsync(RespondFriendRequestRequest request, Guid receiverId)
        {
            try
            {
                // Lấy thông tin lời mời kết bạn
                var friendRequest = await _unitOfWork.FriendRequestRepository.GetFriendRequestAsync(request.SenderId, receiverId);
                if (friendRequest == null)
                {
                    return (RespondFriendRequestEnum.FriendRequestNotFound, null);
                }

                // Kiểm tra xem lời mời đã được xử lý chưa
                if (friendRequest.FriendRequestStatus != FriendRequestStatus.Pending.ToString())
                {
                    return (RespondFriendRequestEnum.AlreadyProcessed, null);
                }

                // Kiểm tra trạng thái phản hồi hợp lệ
                if (request.Status != FriendRequestStatus.Accepted && request.Status != FriendRequestStatus.Rejected)
                {
                    return (RespondFriendRequestEnum.InvalidStatus, null);
                }
          
                friendRequest.FriendRequestStatus = request.Status.ToString();
                
                _unitOfWork.FriendRequestRepository.Update(friendRequest);
               
                if (request.Status == FriendRequestStatus.Accepted)
                {
                    var userRelation1 = new UserRelation
                    {
                        UserId = friendRequest.SenderId,
                        RelatedUserId = friendRequest.ReceiverId,
                        RelationType = UserRelationType.Friend,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    var userRelation2 = new UserRelation
                    {
                        UserId = friendRequest.ReceiverId,
                        RelatedUserId = friendRequest.SenderId,
                        RelationType = UserRelationType.Friend,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _unitOfWork.UserRelationRepository.Add(userRelation1);
                    _unitOfWork.UserRelationRepository.Add(userRelation2);
                }

                var result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
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

    }
}