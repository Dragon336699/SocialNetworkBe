using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum GetUserGroupsEnum
    {
        Success,
        NoGroupsFound,
        Failed
    }

    public static class GetUserGroupsEnumMessage
    {
        public static string GetMessage(this GetUserGroupsEnum status)
        {
            return status switch
            {
                GetUserGroupsEnum.Success => "User groups retrieved successfully.",
                GetUserGroupsEnum.NoGroupsFound => "No groups found for this user.",
                GetUserGroupsEnum.Failed => "Failed to retrieve user groups.",
                _ => "Unknown error."
            };
        }
    }

}
