using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum ApprovePostEnum
    {
        Success,
        PostNotFound,
        PostNotPending,
        GroupNotFound,
        Unauthorized,
        Failed
    }

    public static class ApprovePostEnumMessage
    {
        public static string GetMessage(this ApprovePostEnum status)
        {
            return status switch
            {
                ApprovePostEnum.Success => "Post approved successfully.",
                ApprovePostEnum.PostNotFound => "Post not found.",
                ApprovePostEnum.PostNotPending => "Post is not pending approval.",
                ApprovePostEnum.GroupNotFound => "Group not found.",
                ApprovePostEnum.Unauthorized => "You are not authorized to approve this post.",
                ApprovePostEnum.Failed => "Failed to approve post.",
                _ => "Unknown error."
            };
        }
    }
}
