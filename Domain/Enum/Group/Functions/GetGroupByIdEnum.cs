using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum GetGroupByIdEnum
    {
        Success,
        GroupNotFound,
        Unauthorized,
        Failed
    }

    public static class GetGroupByIdEnumMessage
    {
        public static string GetMessage(this GetGroupByIdEnum status)
        {
            return status switch
            {
                GetGroupByIdEnum.Success => "Group retrieved successfully.",
                GetGroupByIdEnum.GroupNotFound => "Group not found.",
                GetGroupByIdEnum.Unauthorized => "You are not authorized to view this group.",
                GetGroupByIdEnum.Failed => "Failed to retrieve group.",
                _ => "Unknown error."
            };
        }
    }

}
