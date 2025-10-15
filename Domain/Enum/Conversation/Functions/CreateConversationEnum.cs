using System;

namespace Domain.Enum.Conversation.Functions
{
    public enum CreateConversationEnum
    {
        Unauthorized,
        ReceiverNotFound,
        ConversationExists,
        CreateConversationSuccess,
        CreateConversationFailed
    }

    public static class CreateConversationEnumMessage
    {
        public static string GetMessage(this CreateConversationEnum status)
        {
            return status switch
            {
                CreateConversationEnum.Unauthorized => "Unauthorized, please try again!",
                CreateConversationEnum.ReceiverNotFound => "Receiver not found. Please check username again!",
                CreateConversationEnum.ConversationExists => "Conversation already exists.",
                CreateConversationEnum.CreateConversationSuccess => "Conversation created successfully.",
                CreateConversationEnum.CreateConversationFailed => "Failed to create conversation. Please try again!",
                _ => "An unknown error occurred"
            };
        }
    }
}