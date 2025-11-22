using Domain.Contracts.Requests.Group;
using Domain.Contracts.Responses.Group;
using Domain.Enum.Group.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IGroupService
    {
        Task<(CreateGroupEnum, Guid?)> CreateGroupAsync(CreateGroupRequest request, Guid userId);
        Task<(GetAllGroupsEnum, List<GroupDto>?)> GetAllGroupsAsync(int skip = 0, int take = 10);
        Task<(GetGroupByIdEnum, GroupDto?)> GetGroupByIdAsync(Guid groupId, Guid userId);
        Task<(UpdateGroupEnum, GroupDto?)> UpdateGroupAsync(Guid groupId, UpdateGroupRequest request, Guid userId);
        Task<(DeleteGroupEnum, bool)> DeleteGroupAsync(Guid groupId, Guid userId);
        Task<(JoinGroupEnum, bool)> JoinGroupAsync(Guid groupId, Guid userId);
        Task<(LeaveGroupEnum, bool)> LeaveGroupAsync(Guid groupId, Guid userId);
        Task<(GetUserGroupsEnum, List<GroupDto>?)> GetUserGroupsAsync(Guid userId, int skip = 0, int take = 10);
        Task<(PromoteToAdminEnum, GroupUserDto?)> PromoteToAdminAsync(Guid groupId, Guid targetUserId, Guid currentUserId);
        Task<(DemoteAdminEnum, GroupUserDto?)> DemoteAdminAsync(Guid groupId, Guid targetUserId, Guid currentUserId);
        Task<(KickMemberEnum, bool)> KickMemberAsync(Guid groupId, Guid targetUserId, Guid currentUserId);
    }

}
