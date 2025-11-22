using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum PromoteToAdminEnum
    {
        Success,
        GroupNotFound,
        Unauthorized,
        UserNotFound,
        UserNotMember,
        AlreadyAdmin,
        MaxAdminReached,
        CannotPromoteSelf,
        Failed
    }

    public static class PromoteToAdminEnumMessage
    {
        public static string GetMessage(this PromoteToAdminEnum status)
        {
            return status switch
            {
                PromoteToAdminEnum.Success => "User promoted to admin successfully",
                PromoteToAdminEnum.GroupNotFound => "Group not found",
                PromoteToAdminEnum.Unauthorized => "You don't have permission to promote users",
                PromoteToAdminEnum.UserNotFound => "User not found",
                PromoteToAdminEnum.UserNotMember => "User is not a member of this group",
                PromoteToAdminEnum.AlreadyAdmin => "User is already an admin",
                PromoteToAdminEnum.MaxAdminReached => "Maximum number of admins (10) reached",
                PromoteToAdminEnum.CannotPromoteSelf => "Cannot promote yourself",
                PromoteToAdminEnum.Failed => "Failed to promote user to admin",
                _ => "Unknown error occurred"
            };
        }
    }
}
