using Domain.Contracts.Responses.Common;
using Domain.Contracts.Responses.User;
using Domain.Contracts.Responses.UserRelation;
using Domain.Enum.UserRelation.Funtions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IUserRelationService
    {
        Task<FollowUserEnum> FollowUserAsync(Guid currentUserId, Guid targetUserId);
        Task<UnfollowUserEnum> UnfollowUserAsync(Guid currentUserId, Guid targetUserId);
        Task<UnfriendUserEnum> UnfriendUserAsync(Guid currentUserId, Guid targetUserId);
        Task<BlockUserEnum> BlockUserAsync(Guid currentUserId, Guid targetUserId);
        Task<UnblockUserEnum> UnblockUserAsync(Guid currentUserId, Guid targetUserId);
        Task<List<UserDto>> GetFollowersAsync(Guid userId, int skip, int take);
        Task<List<UserDto>> GetFollowingAsync(Guid userId, int skip, int take);
        Task<(List<UserDto> Items, int TotalCount)> GetFriendsAsync(Guid userId, int skip, int take, string? keySearch);
        Task<List<UserDto>> GetFullFriends(Guid userId);
        Task<List<MutualFriendReponse>> GetMutualFriends(Guid userId);
        Task<List<UserDto>> GetBlockedUsersAsync(Guid userId, int skip, int take);
    }
}
