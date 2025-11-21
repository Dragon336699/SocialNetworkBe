using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum LeaveGroupEnum
    {
        GroupNotFound,
        NotMember,
        CannotLeaveAsAdmin,
        LeaveGroupSuccess,
        LeaveGroupFailed
    }

    public static class LeaveGroupEnumMessage
    {
        public static string GetMessage(this LeaveGroupEnum status)
        {
            return status switch
            {
                LeaveGroupEnum.GroupNotFound => "Group not found.",
                LeaveGroupEnum.NotMember => "You are not a member of this group.",
                LeaveGroupEnum.CannotLeaveAsAdmin => "Admin cannot leave the group. Transfer admin role or delete the group first.",
                LeaveGroupEnum.LeaveGroupSuccess => "Left group successfully.",
                LeaveGroupEnum.LeaveGroupFailed => "Failed to leave group.",
                _ => "Unknown error."
            };
        }
    }

}
