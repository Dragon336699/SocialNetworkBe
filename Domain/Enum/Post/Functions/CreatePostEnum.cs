using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Post.Functions
{
    public enum CreatePostEnum
    {
        UserNotFound,
        InvalidContent,
        InvalidImageFormat,     
        FileTooLarge,          
        ImageUploadFailed,     
        CreatePostSuccess,
        CreatePostFailed
    }

    public static class CreatePostEnumMessage
    {
        public static string GetMessage(this CreatePostEnum status)
        {
            return status switch
            {
                CreatePostEnum.UserNotFound => "User not found.",
                CreatePostEnum.InvalidContent => "Invalid post content.",
                CreatePostEnum.InvalidImageFormat => "Invalid image format. Only JPG, PNG, GIF, BMP, WEBP are allowed.", // ✅ Thêm mới
                CreatePostEnum.FileTooLarge => "File size too large. Maximum size is 10MB per file.",                    // ✅ Thêm mới
                CreatePostEnum.ImageUploadFailed => "Failed to upload images to cloud storage.",                        // ✅ Thêm mới
                CreatePostEnum.CreatePostSuccess => "Post created successfully.",
                CreatePostEnum.CreatePostFailed => "Failed to create post.",
                _ => "Unknown error."
            };
        }
    }
}