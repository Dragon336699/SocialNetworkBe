using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Conversation.Functions
{
    public enum ConversationStatus
    {
        CreatedSuccessfully, 
        AlreadyExists,        
        InvalidInput          
    }

    public static class ConversationStatusExtensions
    {        
        public static string GetMessage(this ConversationStatus status)
        {
            return status switch
            {
                ConversationStatus.CreatedSuccessfully => "Conversation created successfully.",
                ConversationStatus.AlreadyExists => "Conversation already exists between these users.",
                ConversationStatus.InvalidInput => "Invalid input: user IDs cannot be the same or unauthorized.",
                _ => "Unknown error."  
            };
        }
    }
}
