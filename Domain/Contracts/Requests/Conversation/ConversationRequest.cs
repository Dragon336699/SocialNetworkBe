using Domain.Enum.Conversation.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Conversation
{
    public class CreateConversationRequest
    {
        [Required]
        public Guid User1Id { get; set; }

        [Required]
        public Guid User2Id { get; set; }

        [Required]
        public ConversationType Type { get; set; }
    }
}
