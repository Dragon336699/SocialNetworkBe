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
        AlreadyRequested,
        JoinRequestSent,
        JoinGroupFailed,
        UserBanned
    }

    public static class JoinGroupEnumMessage
    {
        public static string GetMessage(this JoinGroupEnum status)
        {
            return status switch
            {
                JoinGroupEnum.GroupNotFound => "Group not found.",
                JoinGroupEnum.AlreadyMember => "You are already a member of this group.",
                JoinGroupEnum.AlreadyRequested => "You have already sent a join request to this group.",
                JoinGroupEnum.JoinRequestSent => "Join request sent successfully. Waiting for approval.",
                JoinGroupEnum.JoinGroupFailed => "Failed to join group.",
                JoinGroupEnum.UserBanned => "You are banned from this group and cannot request to join.",
                _ => "Unknown error."
            };
        }
    }

}
