using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Conversation
{
    public class ChangeConversationNameRequest
    {
        [Required]
        public Guid ConversationId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Conversation name must be between 1 and 100 characters")]
        public required string NewConversationName { get; set; }
    }
}
