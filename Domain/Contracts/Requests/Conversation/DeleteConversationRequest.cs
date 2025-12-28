using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Conversation
{
    public class DeleteConversationRequest
    {
        [Required]
        public Guid ConversationId { get; set; }
    }
}
