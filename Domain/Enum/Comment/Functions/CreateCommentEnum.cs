using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Comment.Functions
{
    public enum CreateCommentEnum
    {
        CreateCommentSuccess,
        CreateCommentFailed,
        UserNotFound,
        PostNotFound,
        ParentCommentNotFound,
        InvalidContent,
        InvalidImageFormat,
        FileTooLarge,
        ImageUploadFailed
    }

    public static class CreateCommentEnumMessage
    {
        public static string GetMessage(this CreateCommentEnum status)
        {
            return status switch
            {
                CreateCommentEnum.CreateCommentSuccess => "Comment created successfully.",
                CreateCommentEnum.CreateCommentFailed => "Failed to create comment.",
                CreateCommentEnum.UserNotFound => "User not found.",
                CreateCommentEnum.PostNotFound => "Post not found.",
                CreateCommentEnum.ParentCommentNotFound => "Parent comment not found.",
                CreateCommentEnum.InvalidContent => "Comment content cannot be empty.",
                CreateCommentEnum.InvalidImageFormat => "Invalid image format. Only JPG, PNG, GIF, BMP, WEBP are allowed.",
                CreateCommentEnum.FileTooLarge => "File size too large. Maximum 10MB per file.",
                CreateCommentEnum.ImageUploadFailed => "Failed to upload image.",
                _ => "Unknown error."
            };
        }
    }
}
