using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum JoinGroupEnum
    {
        GroupNotFound,
        AlreadyMember,
        JoinGroupSuccess,
        JoinGroupFailed
    }

    public static class JoinGroupEnumMessage
    {
        public static string GetMessage(this JoinGroupEnum status)
        {
            return status switch
            {
                JoinGroupEnum.GroupNotFound => "Group not found.",
                JoinGroupEnum.AlreadyMember => "You are already a member of this group.",
                JoinGroupEnum.JoinGroupSuccess => "Joined group successfully.",
                JoinGroupEnum.JoinGroupFailed => "Failed to join group.",
                _ => "Unknown error."
            };
        }
    }

}
