using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Comment.Functions
{
    public enum UpdateCommentEnum
    {
        UpdateCommentSuccess,
        UpdateCommentFailed,
        CommentNotFound,
        Unauthorized,
        InvalidContent,
        InvalidImageFormat,
        FileTooLarge,
        ImageUploadFailed
    }

    public static class UpdateCommentEnumMessage
    {
        public static string GetMessage(this UpdateCommentEnum status)
        {
            return status switch
            {
                UpdateCommentEnum.UpdateCommentSuccess => "Comment updated successfully.",
                UpdateCommentEnum.UpdateCommentFailed => "Failed to update comment.",
                UpdateCommentEnum.CommentNotFound => "Comment not found.",
                UpdateCommentEnum.Unauthorized => "You are not authorized to update this comment.",
                UpdateCommentEnum.InvalidContent => "Comment content cannot be empty.",
                UpdateCommentEnum.InvalidImageFormat => "Invalid image format. Only JPG, PNG, GIF, BMP, WEBP are allowed.",
                UpdateCommentEnum.FileTooLarge => "File size too large. Maximum 10MB per file.",
                UpdateCommentEnum.ImageUploadFailed => "Failed to upload image.",
                _ => "Unknown error."
            };
        }
    }
}
