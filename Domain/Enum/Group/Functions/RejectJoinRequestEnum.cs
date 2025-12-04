using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum RejectJoinRequestEnum
    {
        Success,
        GroupNotFound,
        UserNotFound,
        RequestNotFound,
        Unauthorized,
        Failed
    }

    public static class RejectJoinRequestEnumMessage
    {
        public static string GetMessage(this RejectJoinRequestEnum status)
        {
            return status switch
            {
                RejectJoinRequestEnum.Success => "Join request rejected successfully.",
                RejectJoinRequestEnum.GroupNotFound => "Group not found.",
                RejectJoinRequestEnum.UserNotFound => "User not found.",
                RejectJoinRequestEnum.RequestNotFound => "Join request not found.",
                RejectJoinRequestEnum.Unauthorized => "You don't have permission to reject join requests.",
                RejectJoinRequestEnum.Failed => "Failed to reject join request.",
                _ => "Unknown error."
            };
        }
    }
}
