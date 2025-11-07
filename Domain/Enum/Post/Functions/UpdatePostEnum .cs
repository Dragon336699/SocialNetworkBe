using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum UpdatePostEnum
    {
        PostNotFound,
        InvalidContent,
        Unauthorized,
        InvalidImageFormat,
        FileTooLarge,
        ImageUploadFailed,
        UpdatePostSuccess,
        UpdatePostFailed
    }

    public static class UpdatePostEnumMessage
    {
        public static string GetMessage(this UpdatePostEnum status)
        {
            return status switch
            {
                UpdatePostEnum.PostNotFound => "Post not found.",
                UpdatePostEnum.Unauthorized => "You are not authorized to update this post.",
                UpdatePostEnum.InvalidContent => "Invalid post content.",
                UpdatePostEnum.InvalidImageFormat => "Invalid image format. Only JPG, PNG, GIF, BMP, WEBP are allowed.",
                UpdatePostEnum.FileTooLarge => "File size too large. Maximum size is 10MB per file.",
                UpdatePostEnum.ImageUploadFailed => "Failed to upload images to cloud storage.",
                UpdatePostEnum.UpdatePostSuccess => "Post updated successfully.",
                UpdatePostEnum.UpdatePostFailed => "Failed to update post.",
                _ => "Unknown error."
            };
        }
    }
}
