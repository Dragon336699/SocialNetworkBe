using Domain.Enum.Conversation.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Conversation
{
    public class ConversationRequest
    {
      
        [Required]
        public Guid SenderId { get; set; }
       
        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        public required string Content { get; set; }
      
        [Required]
        public ConversationType Type { get; set; }
    }
}
