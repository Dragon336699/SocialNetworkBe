using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum GetPendingJoinRequestsEnum
    {
        Success,
        GroupNotFound,
        Unauthorized,
        NoRequestsFound,
        Failed
    }

    public static class GetPendingJoinRequestsEnumMessage
    {
        public static string GetMessage(this GetPendingJoinRequestsEnum status)
        {
            return status switch
            {
                GetPendingJoinRequestsEnum.Success => "Pending join requests retrieved successfully.",
                GetPendingJoinRequestsEnum.GroupNotFound => "Group not found.",
                GetPendingJoinRequestsEnum.Unauthorized => "You don't have permission to view join requests.",
                GetPendingJoinRequestsEnum.NoRequestsFound => "No pending join requests found.",
                GetPendingJoinRequestsEnum.Failed => "Failed to retrieve join requests.",
                _ => "Unknown error."
            };
        }
    }
}
