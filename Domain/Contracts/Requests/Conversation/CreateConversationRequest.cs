using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Contracts.Requests.Conversation
{
    public class CreateConversationRequest
    {
        [Required]
        public required string ReceiverUserName { get; set; }
    }
}