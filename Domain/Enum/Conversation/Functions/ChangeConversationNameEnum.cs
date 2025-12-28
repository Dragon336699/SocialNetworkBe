using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Conversation.Functions
{
    public enum ChangeConversationNameEnum
    {
        Success,
        ConversationNotFound,
        UserNotInConversation,
        NotGroupConversation,
        Failed
    }

    public static class ChangeConversationNameEnumMessage
    {
        public static string GetMessage(this ChangeConversationNameEnum status)
        {
            return status switch
            {
                ChangeConversationNameEnum.Success => "Conversation name changed successfully.",
                ChangeConversationNameEnum.ConversationNotFound => "Conversation not found.",
                ChangeConversationNameEnum.UserNotInConversation => "You are not a member of this conversation.",
                ChangeConversationNameEnum.NotGroupConversation => "Only group conversations can have their name changed.",
                ChangeConversationNameEnum.Failed => "Failed to change conversation name. Please try again!",
                _ => "An unknown error occurred"
            };
        }
    }
}
