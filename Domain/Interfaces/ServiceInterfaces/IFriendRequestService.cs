using Domain.Contracts.Requests.FriendRequest;
using Domain.Contracts.Responses.Common;
using Domain.Contracts.Responses.FriendRequest;
using Domain.Enum.FriendRequest.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IFriendRequestService
    {
        Task<(SendFriendRequestEnum, FriendRequestDto?)> SendFriendRequestAsync(SendFriendRequestRequest request, Guid senderId);
        Task<(RespondFriendRequestEnum, FriendRequestDto?)> ApproveFriendRequestAsync(RespondFriendRequestRequest request, Guid receiverId);
        Task<RespondFriendRequestEnum> DeclineFriendRequestAsync(RespondFriendRequestRequest request, Guid receiverId);
        Task<List<FriendRequestDto>> GetSentFriendRequestsAsync(Guid senderId, int pageIndex, int pageSize);

        Task<CancelFriendRequestEnum> CancelFriendRequestAsync(CancelFriendRequestRequest request, Guid senderId);
        Task<List<FriendRequestDto>> GetReceivedFriendRequestsAsync(Guid receiverId, int pageIndex, int pageSize);

    }
}
