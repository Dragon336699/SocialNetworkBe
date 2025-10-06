using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.User.Functions
{
    public enum GetOTPEnum
    {
        UserNotFound,
        SpamOTP,
        SentOTP
    }

    public static class GetOTPMessage
    {
        public static string GetMessage(this GetOTPEnum status)
        {
            return status switch
            {
                GetOTPEnum.UserNotFound => "User not found. Please try another email!",
                GetOTPEnum.SpamOTP => "You have requested too many OTPs in a short time, please try again after 30 minutes!",
                GetOTPEnum.SentOTP => "Your OTP was sent, please check your email!",
                _ => "An unknown error occurred"
            };
        }
    }
}
