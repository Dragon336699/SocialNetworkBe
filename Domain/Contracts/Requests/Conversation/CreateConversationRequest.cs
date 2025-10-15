using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Contracts.Requests.Conversation
{
    public class CreateConversationRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public required string ReceiverUserName { get; set; }
    }
}