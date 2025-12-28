using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Conversation.Functions
{
    public enum DeleteConversationEnum
    {
        Success,
        ConversationNotFound,
        UserNotInConversation,
        Failed
    }

    public static class DeleteConversationEnumMessage
    {
        public static string GetMessage(this DeleteConversationEnum status)
        {
            return status switch
            {
                DeleteConversationEnum.Success => "Conversation deleted successfully.",
                DeleteConversationEnum.ConversationNotFound => "Conversation not found.",
                DeleteConversationEnum.UserNotInConversation => "You are not a member of this conversation.",
                DeleteConversationEnum.Failed => "Failed to delete conversation. Please try again!",
                _ => "An unknown error occurred"
            };
        }
    }
}
