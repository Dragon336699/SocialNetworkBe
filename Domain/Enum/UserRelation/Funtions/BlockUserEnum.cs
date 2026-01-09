using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.UserRelation.Funtions
{
    public enum BlockUserEnum
    {
        Success,
        AlreadyBlocked,
        TargetUserNotFound,
        CannotBlockSelf,
        Failed
    }

    public enum UnblockUserEnum
    {
        Success,
        NotBlocked,
        TargetUserNotFound,
        Failed
    }

    public static class BlockUserEnumExtensions
    {
        public static string GetMessage(this BlockUserEnum status)
        {
            return status switch
            {
                BlockUserEnum.Success => "User blocked successfully.",
                BlockUserEnum.AlreadyBlocked => "You have already blocked this user.",
                BlockUserEnum.CannotBlockSelf => "You cannot block yourself.",
                BlockUserEnum.TargetUserNotFound => "User not found.",
                BlockUserEnum.Failed => "Failed to block user.",
                _ => "Unknown error."
            };
        }

        public static string GetMessage(this UnblockUserEnum status)
        {
            return status switch
            {
                UnblockUserEnum.Success => "User unblocked successfully.",
                UnblockUserEnum.NotBlocked => "You have not blocked this user.",
                UnblockUserEnum.TargetUserNotFound => "User not found.",
                UnblockUserEnum.Failed => "Failed to unblock user.",
                _ => "Unknown error."
            };
        }
    }
}
