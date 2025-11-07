using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum DeletePostEnum
    {
        PostNotFound,
        Unauthorized,
        DeletePostSuccess,
        DeletePostFailed
    }

    public static class DeletePostEnumMessage
    {
        public static string GetMessage(this DeletePostEnum status)
        {
            return status switch
            {
                DeletePostEnum.PostNotFound => "Post not found.",
                DeletePostEnum.Unauthorized => "You are not authorized to delete this post.",
                DeletePostEnum.DeletePostSuccess => "Post deleted successfully.",
                DeletePostEnum.DeletePostFailed => "Failed to delete post.",
                _ => "Unknown error."
            };
        }
    }
}
