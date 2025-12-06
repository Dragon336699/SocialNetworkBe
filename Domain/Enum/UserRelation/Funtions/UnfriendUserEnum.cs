using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.UserRelation.Funtions
{
    public enum UnfriendUserEnum
    {
        Success,
        NotFriends,
        Failed
    }

    public static class UnfriendUserEnumExtensions
    {
        public static string GetMessage(this UnfriendUserEnum status)
        {
            return status switch
            {
                UnfriendUserEnum.Success => "Unfriended successfully.",
                UnfriendUserEnum.NotFriends => "You are not friends with this user.",
                UnfriendUserEnum.Failed => "Failed to unfriend user.",
                _ => "Unknown error."
            };
        }
    }
}
