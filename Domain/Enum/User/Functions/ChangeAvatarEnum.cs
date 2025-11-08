namespace Domain.Enum.User.Functions
{
    public enum UpdateAvatarEnum
    {
        Success,
        UserNotFound,
        InvalidImageFormat,
        FileTooLarge,
        UploadFailed,
        UpdateFailed
    }

    public static class UpdateAvatarMessage
    {
        public static string GetMessage(this UpdateAvatarEnum status)
        {
            return status switch
            {
                UpdateAvatarEnum.Success => "Avatar updated successfully",
                UpdateAvatarEnum.UserNotFound => "User not found",
                UpdateAvatarEnum.InvalidImageFormat => "Invalid image format",
                UpdateAvatarEnum.FileTooLarge => "Image too large",
                UpdateAvatarEnum.UploadFailed => "Failed to upload avatar",
                _ => "Failed to update avatar"
            };
        }
    }
}
