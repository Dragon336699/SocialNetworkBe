using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.UserRelation.Funtions
{
    public enum FollowUserEnum
    {
        Success,
        AlreadyFollowing,
        TargetUserNotFound,
        CannotFollowSelf,
        Failed
    }

    public enum UnfollowUserEnum
    {
        Success,
        NotFollowing,
        TargetUserNotFound,
        Failed
    }

    public static class UserRelationMessage
    {
        public static string GetMessage(this FollowUserEnum status) => status switch
        {
            FollowUserEnum.Success => "Follow user successfully.",
            FollowUserEnum.AlreadyFollowing => "You are already following this user.",
            FollowUserEnum.CannotFollowSelf => "You cannot follow yourself.",
            FollowUserEnum.TargetUserNotFound => "User not found.",
            _ => "An error occurred."
        };

        public static string GetMessage(this UnfollowUserEnum status) => status switch
        {
            UnfollowUserEnum.Success => "Unfollow user successfully.",
            UnfollowUserEnum.NotFollowing => "You are not following this user.",
            _ => "An error occurred."
        };
    }
}
