using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum CreateGroupEnum
    {
        UserNotFound,
        InvalidName,
        InvalidDescription,
        InvalidImageFormat,
        FileTooLarge,
        ImageUploadFailed,
        CreateGroupSuccess,
        CreateGroupFailed
    }

    public static class CreateGroupEnumMessage
    {
        public static string GetMessage(this CreateGroupEnum status)
        {
            return status switch
            {
                CreateGroupEnum.UserNotFound => "User not found.",
                CreateGroupEnum.InvalidName => "Invalid group name.",
                CreateGroupEnum.InvalidDescription => "Invalid group description.",
                CreateGroupEnum.InvalidImageFormat => "Invalid image format. Only jpg, jpeg, png, gif, bmp, webp are allowed.",
                CreateGroupEnum.FileTooLarge => "Image file is too large. Maximum size is 10MB.",
                CreateGroupEnum.ImageUploadFailed => "Failed to upload image.",
                CreateGroupEnum.CreateGroupSuccess => "Group created successfully.",
                CreateGroupEnum.CreateGroupFailed => "Failed to create group.",
                _ => "Unknown error."
            };
        }
    }

}
