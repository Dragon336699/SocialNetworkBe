using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Conversation.Functions
{
    public enum ChangeNicknameEnum
    {
        Success,
        ConversationNotFound,
        UserNotInConversation,
        Failed
    }

    public static class ChangeNicknameEnumMessage
    {
        public static string GetMessage(this ChangeNicknameEnum status)
        {
            return status switch
            {
                ChangeNicknameEnum.Success => "Nickname changed successfully.",
                ChangeNicknameEnum.ConversationNotFound => "Conversation not found.",
                ChangeNicknameEnum.UserNotInConversation => "User is not in this conversation.",
                ChangeNicknameEnum.Failed => "Failed to change nickname. Please try again!",
                _ => "An unknown error occurred"
            };
        }
    }
}
