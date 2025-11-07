using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.FriendRequest.Functions
{
    public enum FriendRequestStatus
    {
        Pending,
        Accepted,
        Rejected,
        Cancelled
    }

    public static class FriendRequestStatusExtensions
    {
        public static string GetDisplayName(this FriendRequestStatus status)
        {
            return status switch
            {
                FriendRequestStatus.Pending => "Pending",
                FriendRequestStatus.Accepted => "Accepted",
                FriendRequestStatus.Rejected => "Rejected",
                FriendRequestStatus.Cancelled => "Cancelled",
                _ => "Unknown"
            };
        }
    }
}
