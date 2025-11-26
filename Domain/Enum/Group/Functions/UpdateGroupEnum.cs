using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum UpdateGroupEnum
    {
        GroupNotFound,
        Unauthorized,
        InvalidName,
        InvalidDescription,
        InvalidImageFormat,
        FileTooLarge,
        ImageUploadFailed,
        UpdateGroupSuccess,
        UpdateGroupFailed
    }

    public static class UpdateGroupEnumMessage
    {
        public static string GetMessage(this UpdateGroupEnum status)
        {
            return status switch
            {
                UpdateGroupEnum.GroupNotFound => "Group not found.",
                UpdateGroupEnum.Unauthorized => "You are not authorized to update this group.",
                UpdateGroupEnum.InvalidName => "Invalid group name.",
                UpdateGroupEnum.InvalidDescription => "Invalid group description.",
                UpdateGroupEnum.InvalidImageFormat => "Invalid image format. Only jpg, jpeg, png, gif, bmp, webp are allowed.",
                UpdateGroupEnum.FileTooLarge => "Image file is too large. Maximum size is 10MB.",
                UpdateGroupEnum.ImageUploadFailed => "Failed to upload image.",
                UpdateGroupEnum.UpdateGroupSuccess => "Group updated successfully.",
                UpdateGroupEnum.UpdateGroupFailed => "Failed to update group.",
                _ => "Unknown error."
            };
        }
    }
}
