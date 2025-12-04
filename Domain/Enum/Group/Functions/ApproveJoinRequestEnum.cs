using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum ApproveJoinRequestEnum
    {
        Success,
        GroupNotFound,
        UserNotFound,
        RequestNotFound,
        AlreadyMember,
        Unauthorized,
        Failed
    }

    public static class ApproveJoinRequestEnumMessage
    {
        public static string GetMessage(this ApproveJoinRequestEnum status)
        {
            return status switch
            {
                ApproveJoinRequestEnum.Success => "Join request approved successfully.",
                ApproveJoinRequestEnum.GroupNotFound => "Group not found.",
                ApproveJoinRequestEnum.UserNotFound => "User not found.",
                ApproveJoinRequestEnum.RequestNotFound => "Join request not found.",
                ApproveJoinRequestEnum.AlreadyMember => "User is already a member.",
                ApproveJoinRequestEnum.Unauthorized => "You don't have permission to approve join requests.",
                ApproveJoinRequestEnum.Failed => "Failed to approve join request.",
                _ => "Unknown error."
            };
        }
    }
}
