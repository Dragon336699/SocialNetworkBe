using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum BanMemberEnum
    {
        Success,
        GroupNotFound,
        UserNotFound,
        TargetUserNotMember,
        Unauthorized,
        CannotBanSelf,
        CannotBanSuperAdmin,
        CannotBanAdmin,
        AdminCannotBanAdmin,
        AlreadyBanned,
        Failed
    }

    public static class BanMemberEnumExtensions
    {
        public static string GetMessage(this BanMemberEnum status)
        {
            return status switch
            {
                BanMemberEnum.Success => "Member banned successfully.",
                BanMemberEnum.GroupNotFound => "Group not found.",
                BanMemberEnum.UserNotFound => "User not found.",
                BanMemberEnum.TargetUserNotMember => "Target user is not a member of this group.",
                BanMemberEnum.Unauthorized => "You are not authorized to ban members.",
                BanMemberEnum.CannotBanSelf => "You cannot ban yourself.",
                BanMemberEnum.CannotBanSuperAdmin => "Cannot ban the Super Administrator.",
                BanMemberEnum.CannotBanAdmin => "Cannot ban an Administrator.",
                BanMemberEnum.AdminCannotBanAdmin => "Administrator cannot ban another Administrator.",
                BanMemberEnum.AlreadyBanned => "This user is already banned.",
                BanMemberEnum.Failed => "Failed to ban member.",
                _ => "Unknown error."
            };
        }
    }
}
