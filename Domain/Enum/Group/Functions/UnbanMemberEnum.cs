using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum UnbanMemberEnum
    {
        Success,
        GroupNotFound,
        UserNotFound,
        UserNotBanned,
        Unauthorized,
        Failed
    }

    public static class UnbanMemberEnumExtensions
    {
        public static string GetMessage(this UnbanMemberEnum status)
        {
            return status switch
            {
                UnbanMemberEnum.Success => "Member unbanned successfully.",
                UnbanMemberEnum.GroupNotFound => "Group not found.",
                UnbanMemberEnum.UserNotFound => "User not found.",
                UnbanMemberEnum.UserNotBanned => "This user is not banned.",
                UnbanMemberEnum.Unauthorized => "You are not authorized to unban members.",
                UnbanMemberEnum.Failed => "Failed to unban member.",
                _ => "Unknown error."
            };
        }
    }
}
