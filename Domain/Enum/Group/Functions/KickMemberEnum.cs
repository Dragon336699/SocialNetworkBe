using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum KickMemberEnum
    {
        Success,
        GroupNotFound,
        UserNotFound,
        TargetUserNotMember,
        Unauthorized,
        CannotKickSelf,
        CannotKickSuperAdmin,
        AdminCannotKickAdmin,
        Failed
    }

    public static class KickMemberEnumExtensions
    {
        public static string GetMessage(this KickMemberEnum status)
        {
            return status switch
            {
                KickMemberEnum.Success => "User has been kicked from the group successfully",
                KickMemberEnum.GroupNotFound => "Group not found",
                KickMemberEnum.UserNotFound => "User not found",
                KickMemberEnum.TargetUserNotMember => "Target user is not a member of this group",
                KickMemberEnum.Unauthorized => "You don't have permission to kick members",
                KickMemberEnum.CannotKickSelf => "You cannot kick yourself from the group",
                KickMemberEnum.CannotKickSuperAdmin => "Cannot kick the Super Administrator",
                KickMemberEnum.AdminCannotKickAdmin => "Administrator cannot kick another Administrator",
                KickMemberEnum.Failed => "Failed to kick user from the group",
                _ => "Unknown error occurred"
            };
        }
    }
}
