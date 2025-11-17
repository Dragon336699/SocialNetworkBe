using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Comment.Functions
{
    public enum DeleteCommentEnum
    {
        DeleteCommentSuccess,
        DeleteCommentFailed,
        CommentNotFound,
        Unauthorized
    }

    public static class DeleteCommentEnumMessage
    {
        public static string GetMessage(this DeleteCommentEnum status)
        {
            return status switch
            {
                DeleteCommentEnum.DeleteCommentSuccess => "Comment deleted successfully.",
                DeleteCommentEnum.DeleteCommentFailed => "Failed to delete comment.",
                DeleteCommentEnum.CommentNotFound => "Comment not found.",
                DeleteCommentEnum.Unauthorized => "You are not authorized to delete this comment.",
                _ => "Unknown error."
            };
        }
    }
}
