using Domain.Enum.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Role.Functions
{
    public enum CreateRoleReturn
    {
        CreateRoleSuccess,
        CreateRoleFailure,
    }

    public static class UserRegistrationReturnMessage
    {
        public static string GetMessage(this CreateRoleReturn status)
        {
            return status switch
            {
                CreateRoleReturn.CreateRoleSuccess => "New role created successfully",
                CreateRoleReturn.CreateRoleFailure => "Failed to create new role",
                _ => "An unknown error occurred"
            };
        }
    }
}
