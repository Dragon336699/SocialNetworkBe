using Domain.Entities;
using Domain.Enum.FriendRequest.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IFriendRequestRepository : IGenericRepository<FriendRequest>
    {
        Task<FriendRequest?> GetFriendRequestAsync(Guid senderId, Guid receiverId);
        Task<bool> AreFriendsAsync(Guid userId1, Guid userId2);
        Task<(List<FriendRequest> Items, int TotalCount)> GetSentFriendRequestsAsync(Guid senderId, int skip, int take, string? keySearch);
        Task<(List<FriendRequest> Items, int TotalCount)> GetReceivedFriendRequestsAsync(Guid receiverId, int skip, int take, string? keySearch);
    }
}
