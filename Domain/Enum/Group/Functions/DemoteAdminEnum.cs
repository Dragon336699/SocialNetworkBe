using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum DemoteAdminEnum
    {
        Success,
        GroupNotFound,
        Unauthorized,
        UserNotFound,
        UserNotAdmin,
        CannotDemoteSuperAdmin,
        CannotDemoteSelf,
        Failed
    }

    public static class DemoteAdminEnumMessage
    {
        public static string GetMessage(this DemoteAdminEnum status)
        {
            return status switch
            {
                DemoteAdminEnum.Success => "Admin demoted successfully",
                DemoteAdminEnum.GroupNotFound => "Group not found",
                DemoteAdminEnum.Unauthorized => "Only super admin can demote other admins",
                DemoteAdminEnum.UserNotFound => "User not found",
                DemoteAdminEnum.UserNotAdmin => "User is not an admin",
                DemoteAdminEnum.CannotDemoteSuperAdmin => "Cannot demote the super admin",
                DemoteAdminEnum.CannotDemoteSelf => "Cannot demote yourself",
                DemoteAdminEnum.Failed => "Failed to demote admin",
                _ => "Unknown error occurred"
            };
        }
    }
}
