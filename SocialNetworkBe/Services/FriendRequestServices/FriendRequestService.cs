using Domain.Contracts.Requests.FriendRequest;
using Domain.Contracts.Responses.Common;
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
        public async Task<PagedResponse<FriendRequestDto>> GetSentFriendRequestsAsync(Guid senderId, int pageIndex, int pageSize)
        {
            // Đảm bảo pageIndex >= 1
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            // Gọi Repository lấy dữ liệu và tổng số
            var (friendRequests, totalCount) = await _unitOfWork.FriendRequestRepository.GetSentFriendRequestsAsync(senderId, pageIndex, pageSize);

            // Map sang DTO
            var requestDtos = friendRequests.Select(fr => new FriendRequestDto
            {
                SenderId = fr.SenderId,
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

            // Trả về PagedResponse
            return new PagedResponse<FriendRequestDto>(requestDtos, pageIndex, pageSize, totalCount);
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

    }
}