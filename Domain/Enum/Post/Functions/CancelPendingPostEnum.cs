using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum CancelPendingPostEnum
    {
        Success,
        PostNotFound,
        PostNotPending,
        Unauthorized,
        Failed
    }

    public static class CancelPendingPostEnumMessage
    {
        public static string GetMessage(this CancelPendingPostEnum status)
        {
            return status switch
            {
                CancelPendingPostEnum.Success => "Pending post cancelled successfully.",
                CancelPendingPostEnum.PostNotFound => "Post not found.",
                CancelPendingPostEnum.PostNotPending => "Post is not pending approval.",
                CancelPendingPostEnum.Unauthorized => "You are not authorized to cancel this post.",
                CancelPendingPostEnum.Failed => "Failed to cancel pending post.",
                _ => "Unknown error."
            };
        }
    }
}
