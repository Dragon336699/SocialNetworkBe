using System;

namespace Domain.Enum.Conversation.Functions
{
    public enum CreateConversationEnum
    {     
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
                CreateConversationEnum.ReceiverNotFound => "Receiver not found. Please check username again!",
                CreateConversationEnum.ConversationExists => "Conversation already exists.",
                CreateConversationEnum.CreateConversationSuccess => "Conversation created successfully.",
                CreateConversationEnum.CreateConversationFailed => "Failed to create conversation. Please try again!",
                _ => "An unknown error occurred"
            };
        }
    }
}