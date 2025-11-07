using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum GetPostByIdEnum
    {
        Success,
        PostNotFound,
        Unauthorized,
        Failed
    }

    public static class GetPostByIdEnumMessage
    {
        public static string GetMessage(this GetPostByIdEnum status)
        {
            return status switch
            {
                GetPostByIdEnum.Success => "Post retrieved successfully.",
                GetPostByIdEnum.Unauthorized => "You are not authorized to view this post.",
                GetPostByIdEnum.PostNotFound => "Post not found.",
                GetPostByIdEnum.Failed => "Failed to retrieve post.",
                _ => "Unknown error."
            };
        }
    }
}
