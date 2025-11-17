using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Comment.Functions
{
    public enum GetCommentsEnum
    {
        Success,
        PostNotFound,
        NoCommentsFound,
        Failed
    }

    public static class GetCommentsEnumMessage
    {
        public static string GetMessage(this GetCommentsEnum status)
        {
            return status switch
            {
                GetCommentsEnum.Success => "Comments retrieved successfully.",
                GetCommentsEnum.PostNotFound => "Post not found.",
                GetCommentsEnum.NoCommentsFound => "No comments found.",
                GetCommentsEnum.Failed => "Failed to retrieve comments.",
                _ => "Unknown error."
            };
        }
    }
}
