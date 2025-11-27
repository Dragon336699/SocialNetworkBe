using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.FriendRequest.Functions
{
    public enum CancelFriendRequestEnum
    {
        Success,
        RequestNotFound,
        NotPending,
        Failed,
        Unauthorized
    }

    public static class CancelFriendRequestEnumExtensions
    {
        public static string GetMessage(this CancelFriendRequestEnum status)
        {
            return status switch
            {
                CancelFriendRequestEnum.Success => "Friend request cancelled successfully.",
                CancelFriendRequestEnum.RequestNotFound => "Friend request not found.",
                CancelFriendRequestEnum.NotPending => "Cannot cancel a processed request.",
                CancelFriendRequestEnum.Unauthorized => "You are not authorized to cancel this request.",
                CancelFriendRequestEnum.Failed => "Failed to cancel friend request.",
                _ => "Unknown error."
            };
        }
    }
}
