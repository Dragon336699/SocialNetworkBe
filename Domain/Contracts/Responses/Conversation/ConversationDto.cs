using Domain.Entities;
using Domain.Enum.Conversation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Conversation
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public ConversationType Type { get; set; }
        public string? ConversationName { get; set; }
        public List<ConversationUser>? ConversationUsers { get; set; }
    }
}
