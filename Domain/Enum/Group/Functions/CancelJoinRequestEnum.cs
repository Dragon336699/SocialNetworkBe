using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum CancelJoinRequestEnum
    {
        Success,
        GroupNotFound,
        RequestNotFound,
        Failed
    }

    public static class CancelJoinRequestEnumMessage
    {
        public static string GetMessage(this CancelJoinRequestEnum status)
        {
            return status switch
            {
                CancelJoinRequestEnum.Success => "Join request cancelled successfully.",
                CancelJoinRequestEnum.GroupNotFound => "Group not found.",
                CancelJoinRequestEnum.RequestNotFound => "Join request not found.",
                CancelJoinRequestEnum.Failed => "Failed to cancel join request.",
                _ => "Unknown error."
            };
        }
    }
}
