using Domain.Enum.Conversation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.ConversationUser
{
    public class GetConversationUserRequest
    {
        public Guid SenderId { get; set; }
        public Guid ConversationId { get; set; }
        public ConversationType ConversationType { get; set; }
    }
}
