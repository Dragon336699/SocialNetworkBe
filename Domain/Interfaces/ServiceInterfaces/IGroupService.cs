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
        Task<(GetAllGroupsEnum, List<GroupDto>?)> GetAllGroupsAsync(Guid userId, int skip = 0, int take = 10);
        Task<(GetGroupByIdEnum, GroupDto?)> GetGroupByIdAsync(Guid groupId, Guid userId);
        Task<(UpdateGroupEnum, GroupDto?)> UpdateGroupAsync(Guid groupId, UpdateGroupRequest request, Guid userId);
        Task<(DeleteGroupEnum, bool)> DeleteGroupAsync(Guid groupId, Guid userId);
        Task<(JoinGroupEnum, bool)> JoinGroupAsync(Guid groupId, Guid userId);
        Task<(ApproveJoinRequestEnum, GroupUserDto?)> ApproveJoinRequestAsync(Guid groupId, Guid targetUserId, Guid currentUserId);
        Task<(RejectJoinRequestEnum, bool)> RejectJoinRequestAsync(Guid groupId, Guid targetUserId, Guid currentUserId);
        Task<(CancelJoinRequestEnum, bool)> CancelJoinRequestAsync(Guid groupId, Guid userId);
        Task<(GetPendingJoinRequestsEnum, List<GroupUserDto>?)> GetPendingJoinRequestsAsync(Guid groupId, Guid currentUserId, int skip = 0, int take = 10);
        Task<(LeaveGroupEnum, bool)> LeaveGroupAsync(Guid groupId, Guid userId);
        Task<(GetUserGroupsEnum, List<GroupDto>?)> GetUserGroupsAsync(Guid userId, int skip = 0, int take = 10);
        Task<(SearchMyGroupsEnum, List<GroupDto>?)> SearchMyGroupsAsync(Guid userId, string searchTerm, int skip = 0, int take = 10);
        Task<(PromoteToAdminEnum, GroupUserDto?)> PromoteToAdminAsync(Guid groupId, Guid targetUserId, Guid currentUserId);
        Task<(DemoteAdminEnum, GroupUserDto?)> DemoteAdminAsync(Guid groupId, Guid targetUserId, Guid currentUserId);
        Task<(KickMemberEnum, bool)> KickMemberAsync(Guid groupId, Guid targetUserId, Guid currentUserId);
        Task<(InviteMemberEnum, bool)> InviteMemberAsync(Guid groupId, Guid targetUserId, Guid inviterId);
        Task<(AcceptGroupInviteEnum, GroupDto?)> AcceptGroupInviteAsync(Guid groupId, Guid userId);
        Task<(RejectGroupInviteEnum, bool)> RejectGroupInviteAsync(Guid groupId, Guid userId);
        Task<(GetMyGroupInvitationsEnum, List<GroupInvitationDto>?)> GetMyGroupInvitationsAsync(Guid userId, int skip = 0, int take = 10);
        Task<(BanMemberEnum, bool)> BanMemberAsync(Guid groupId, Guid targetUserId, Guid currentUserId);
        Task<(UnbanMemberEnum, bool)> UnbanMemberAsync(Guid groupId, Guid targetUserId, Guid currentUserId);
        Task<(GetBannedMembersEnum, List<GroupUserDto>?)> GetBannedMembersAsync(Guid groupId, Guid currentUserId, int skip = 0, int take = 10);
    }

}
