using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum GetMyPendingPostsEnum
    {
        Success,
        NoPendingPosts,
        Failed
    }

    public static class GetMyPendingPostsEnumMessage
    {
        public static string GetMessage(this GetMyPendingPostsEnum status)
        {
            return status switch
            {
                GetMyPendingPostsEnum.Success => "Your pending posts retrieved successfully.",
                GetMyPendingPostsEnum.NoPendingPosts => "No pending posts found.",
                GetMyPendingPostsEnum.Failed => "Failed to retrieve your pending posts.",
                _ => "Unknown error."
            };
        }
    }
}
