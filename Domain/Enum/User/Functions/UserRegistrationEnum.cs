using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.User
{
    public enum UserRegistrationEnum
    {
        CreatedUserFailure,
        RegistrationSuccess,
        RegistrationFailure,
    }

    public static class UserRegistrationReturnMessage
    {
        public static string GetMessage(this UserRegistrationEnum status)
        {
            return status switch
            {
                UserRegistrationEnum.CreatedUserFailure => "Create User failure",
                UserRegistrationEnum.RegistrationSuccess => "Registration new user successfully",
                UserRegistrationEnum.RegistrationFailure => "Registration new user failure",
                _ => "An unknown error occured"
            };
        }
    }
}
