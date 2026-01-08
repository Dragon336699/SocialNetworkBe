using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum RejectPostEnum
    {
        Success,
        PostNotFound,
        PostNotPending,
        GroupNotFound,
        Unauthorized,
        Failed
    }

    public static class RejectPostEnumMessage
    {
        public static string GetMessage(this RejectPostEnum status)
        {
            return status switch
            {
                RejectPostEnum.Success => "Post rejected successfully.",
                RejectPostEnum.PostNotFound => "Post not found.",
                RejectPostEnum.PostNotPending => "Post is not pending approval.",
                RejectPostEnum.GroupNotFound => "Group not found.",
                RejectPostEnum.Unauthorized => "You are not authorized to reject this post.",
                RejectPostEnum.Failed => "Failed to reject post.",
                _ => "Unknown error."
            };
        }
    }
}
