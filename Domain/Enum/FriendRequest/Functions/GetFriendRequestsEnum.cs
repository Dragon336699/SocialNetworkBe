using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.FriendRequest.Functions
{
    public enum GetFriendRequestsEnum
    {
        Success,
        NoRequestsFound,
        Failed
    }

    public static class GetFriendRequestsEnumMessage
    {
        public static string GetMessage(this GetFriendRequestsEnum status)
        {
            return status switch
            {
                GetFriendRequestsEnum.Success => "Friend requests retrieved successfully.",
                GetFriendRequestsEnum.NoRequestsFound => "No friend requests found.",
                GetFriendRequestsEnum.Failed => "Failed to retrieve friend requests.",
                _ => "Unknown error."
            };
        }
    }
}
