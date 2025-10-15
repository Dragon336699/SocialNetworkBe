using System;

namespace Domain.Contracts.Responses.Conversation
{
    public class CreateConversationResponse
    {    
        public Guid? ConversationId { get; set; }
        public required string Message { get; set; }
    }
}