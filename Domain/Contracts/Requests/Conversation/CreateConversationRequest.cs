using Domain.Enum.Conversation.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Contracts.Requests.Conversation
{
    public class CreateConversationRequest
    {
        [Required]
        public required ConversationType ConversationType { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least 2 users are required for a conversation")]
        public required List<Guid> UserIds { get; set; }
    }
}