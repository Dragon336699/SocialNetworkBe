using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum GetBannedMembersEnum
    {
        Success,
        GroupNotFound,
        Unauthorized,
        NoBannedMembers,
        Failed
    }

    public static class GetBannedMembersEnumExtensions
    {
        public static string GetMessage(this GetBannedMembersEnum status)
        {
            return status switch
            {
                GetBannedMembersEnum.Success => "Banned members retrieved successfully.",
                GetBannedMembersEnum.GroupNotFound => "Group not found.",
                GetBannedMembersEnum.Unauthorized => "You are not authorized to view banned members.",
                GetBannedMembersEnum.NoBannedMembers => "No banned members found.",
                GetBannedMembersEnum.Failed => "Failed to get banned members.",
                _ => "Unknown error."
            };
        }
    }
}
