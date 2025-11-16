using Domain.Contracts.Requests.FriendRequest;
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
        Task<bool> CancelFriendRequestAsync(Guid senderId, SendFriendRequestRequest receiverId);
        Task<(GetFriendRequestsEnum, List<FriendRequestDto>)> GetSentFriendRequestsAsync(Guid senderId, int skip = 0, int take = 10);
        Task<(RespondFriendRequestEnum, FriendRequestDto?)> RespondFriendRequestAsync(RespondFriendRequestRequest request, Guid receiverId);

    }
}
