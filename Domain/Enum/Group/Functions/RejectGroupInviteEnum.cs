using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum RejectGroupInviteEnum
    {
        Success,
        GroupNotFound,
        InvitationNotFound,
        Failed
    }

    public static class RejectGroupInviteEnumMessage
    {
        public static string GetMessage(this RejectGroupInviteEnum status)
        {
            return status switch
            {
                RejectGroupInviteEnum.Success => "Invitation rejected successfully.",
                RejectGroupInviteEnum.GroupNotFound => "Group not found.",
                RejectGroupInviteEnum.InvitationNotFound => "Invitation not found or expired.",
                RejectGroupInviteEnum.Failed => "Failed to reject invitation.",
                _ => "Unknown error."
            };
        }
    }
}
