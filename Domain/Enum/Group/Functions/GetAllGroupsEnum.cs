using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum GetAllGroupsEnum
    {
        Success,
        NoGroupsFound,
        Failed
    }

    public static class GetAllGroupsEnumMessage
    {
        public static string GetMessage(this GetAllGroupsEnum status)
        {
            return status switch
            {
                GetAllGroupsEnum.Success => "Groups retrieved successfully.",
                GetAllGroupsEnum.NoGroupsFound => "No groups found.",
                GetAllGroupsEnum.Failed => "Failed to retrieve groups.",
                _ => "Unknown error."
            };
        }
    }

}
