using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum GetMyGroupInvitationsEnum
    {
        Success,
        NoInvitationsFound,
        Failed
    }

    public static class GetMyGroupInvitationsEnumMessage
    {
        public static string GetMessage(this GetMyGroupInvitationsEnum status)
        {
            return status switch
            {
                GetMyGroupInvitationsEnum.Success => "Invitations retrieved successfully.",
                GetMyGroupInvitationsEnum.NoInvitationsFound => "No pending invitations found.",
                GetMyGroupInvitationsEnum.Failed => "Failed to retrieve invitations.",
                _ => "Unknown error."
            };
        }
    }
}
