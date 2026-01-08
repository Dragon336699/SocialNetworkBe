using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum InviteMemberEnum
    {
        Success,
        GroupNotFound,
        InviterNotMember,
        TargetUserNotFound,
        AlreadyMember,
        AlreadyInvited,
        CannotInviteSelf,
        UserBanned,
        Failed
    }

    public static class InviteMemberEnumMessage
    {
        public static string GetMessage(this InviteMemberEnum status)
        {
            return status switch
            {
                InviteMemberEnum.Success => "Invitation sent successfully.",
                InviteMemberEnum.GroupNotFound => "Group not found.",
                InviteMemberEnum.InviterNotMember => "You must be a member to invite others.",
                InviteMemberEnum.TargetUserNotFound => "User not found.",
                InviteMemberEnum.AlreadyMember => "User is already a member.",
                InviteMemberEnum.AlreadyInvited => "User has already been invited.",
                InviteMemberEnum.CannotInviteSelf => "You cannot invite yourself.",
                InviteMemberEnum.UserBanned => "Cannot invite banned user.",
                InviteMemberEnum.Failed => "Failed to send invitation.",
                _ => "Unknown error."
            };
        }
    }
}
