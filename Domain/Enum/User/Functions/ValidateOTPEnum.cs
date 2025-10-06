using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.User.Functions
{
    public enum ValidateOTPEnum
    {
        UserNotFound,
        CorrectOTP,
        IncorrectOTP
    }

    public static class ValidateOTPMessage
    {
        public static string GetMessage(this ValidateOTPEnum status)
        {
            return status switch
            {
                ValidateOTPEnum.UserNotFound => "User Not Found.",
                ValidateOTPEnum.IncorrectOTP => "Your OTP is incorrect. Please try again.",
                ValidateOTPEnum.CorrectOTP => "Your OTP is correct. Please enter your new password.",
                _ => "An unknown error occurred"
            };
        }
    }
}
