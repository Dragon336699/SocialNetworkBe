using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum DeleteGroupEnum
    {
        GroupNotFound,
        Unauthorized,
        DeleteGroupSuccess,
        DeleteGroupFailed
    }

    public static class DeleteGroupEnumMessage
    {
        public static string GetMessage(this DeleteGroupEnum status)
        {
            return status switch
            {
                DeleteGroupEnum.GroupNotFound => "Group not found.",
                DeleteGroupEnum.Unauthorized => "You are not authorized to delete this group.",
                DeleteGroupEnum.DeleteGroupSuccess => "Group deleted successfully.",
                DeleteGroupEnum.DeleteGroupFailed => "Failed to delete group.",
                _ => "Unknown error."
            };
        }
    }

}
