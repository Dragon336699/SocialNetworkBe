using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.User.Functions
{
    public enum LoginEnum
    {
        LoginSucceded,
        LoginFailed,
        EmailUnConfirmed
    }

    public static class LoginMessage
    {
        public static string GetMessage(this LoginEnum status)
        {
            return status switch
            {
                LoginEnum.LoginSucceded => "Login Successful",
                LoginEnum.LoginFailed => "Login failed",
                LoginEnum.EmailUnConfirmed => "Your email has not been confirmed, please check your email to validate your account.",
                _ => "An unknown error occurred"
            };
        }
    }
}
