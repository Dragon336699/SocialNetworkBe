using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum AcceptGroupInviteEnum
    {
        Success,
        GroupNotFound,
        InvitationNotFound,
        AlreadyMember,
        Failed
    }

    public static class AcceptGroupInviteEnumMessage
    {
        public static string GetMessage(this AcceptGroupInviteEnum status)
        {
            return status switch
            {
                AcceptGroupInviteEnum.Success => "Invitation accepted successfully.",
                AcceptGroupInviteEnum.GroupNotFound => "Group not found.",
                AcceptGroupInviteEnum.InvitationNotFound => "Invitation not found or expired.",
                AcceptGroupInviteEnum.AlreadyMember => "You are already a member.",
                AcceptGroupInviteEnum.Failed => "Failed to accept invitation.",
                _ => "Unknown error."
            };
        }
    }
}
