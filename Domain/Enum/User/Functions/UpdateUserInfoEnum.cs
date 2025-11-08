using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.User.Functions
{
    public enum UpdateUserInfoEnum
    {
        Success,
        UserNotFound,
        InvalidEmail,
        UpdateFailed,
        UploadFailed
    }

    public static class UpdateUserInfoMessage
    {
        public static string GetMessage(this UpdateUserInfoEnum status)
        {
            return status switch
            {
                UpdateUserInfoEnum.Success => "User info updated successfully",
                UpdateUserInfoEnum.UserNotFound => "User not found",
                UpdateUserInfoEnum.InvalidEmail => "Invalid email format",
                UpdateUserInfoEnum.UpdateFailed => "Failed to update user info",
                UpdateUserInfoEnum.UploadFailed => "Failed to upload avatar",
                _ => "Unknown error"
            };
        }
    }
}
