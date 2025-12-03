using Domain.Contracts.Responses.Common;
using Domain.Contracts.Responses.User;
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

        Task<PagedResponse<UserDto>> GetFollowersAsync(Guid userId, int pageIndex, int pageSize);
        Task<PagedResponse<UserDto>> GetFollowingAsync(Guid userId, int pageIndex, int pageSize);
        Task<PagedResponse<UserDto>> GetFriendsAsync(Guid userId, int pageIndex, int pageSize);
    }
}
