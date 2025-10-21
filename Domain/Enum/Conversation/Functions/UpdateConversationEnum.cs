using System;

namespace Domain.Enum.Conversation.Functions
{
    public enum UpdateConversationEnum
    {
        ConversationNotFound,
        ConversationUserNotFound,
        UpdateConversationSuccess,
        UpdateConversationFailed
    }

    public static class UpdateConversationEnumMessage
    {
        public static string GetMessage(this UpdateConversationEnum status)
        {
            return status switch
            {
                UpdateConversationEnum.ConversationNotFound => "Conversation not found.",
                UpdateConversationEnum.ConversationUserNotFound => "User is not part of the conversation.",
                UpdateConversationEnum.UpdateConversationSuccess => "Conversation updated successfully.",
                UpdateConversationEnum.UpdateConversationFailed => "Failed to update conversation. Please try again!",
                _ => "An unknown error occurred"
            };
        }
    }
}