using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum GetPendingPostsEnum
    {
        Success,
        GroupNotFound,
        Unauthorized,
        NoPendingPosts,
        Failed
    }

    public static class GetPendingPostsEnumMessage
    {
        public static string GetMessage(this GetPendingPostsEnum status)
        {
            return status switch
            {
                GetPendingPostsEnum.Success => "Pending posts retrieved successfully.",
                GetPendingPostsEnum.GroupNotFound => "Group not found.",
                GetPendingPostsEnum.Unauthorized => "You are not authorized to view pending posts.",
                GetPendingPostsEnum.NoPendingPosts => "No pending posts found.",
                GetPendingPostsEnum.Failed => "Failed to retrieve pending posts.",
                _ => "Unknown error."
            };
        }
    }
}
