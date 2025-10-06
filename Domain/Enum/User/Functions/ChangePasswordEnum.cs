using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.User.Functions
{
    public enum ChangePasswordEnum
    {
        UserNotFound,
        OldPasswordIncorrect,
        NewPasswordInvalid,
        DuplicatePassword,
        ChangePasswordSuccess
    }

    public static class ChangePasswordMessage
    {
        public static string GetMessage (this ChangePasswordEnum status)
        {
            return status switch
            {
                ChangePasswordEnum.UserNotFound => "User not found!",
                ChangePasswordEnum.OldPasswordIncorrect => "Your old password is incorrect. Please try again!",
                ChangePasswordEnum.NewPasswordInvalid => "Password must contain at least 8 characters, including one uppercase letter, one number, and one special character.",
                ChangePasswordEnum.DuplicatePassword => "Your new password have to be different with your old password.",
                ChangePasswordEnum.ChangePasswordSuccess => "Your password changed successfully!",
                _ => "An unknown error occurred"
            };
        }
    }
}
