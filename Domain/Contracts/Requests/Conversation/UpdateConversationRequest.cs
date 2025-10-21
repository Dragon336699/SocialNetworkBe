using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Contracts.Requests.Conversation
{
    public class UpdateConversationRequest
    {
        [Required]
        public Guid ConversationId { get; set; }

        public string? NickName { get; set; }

        public string? DraftMessage { get; set; }
    }
}