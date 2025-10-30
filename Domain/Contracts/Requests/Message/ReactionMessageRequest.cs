using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Message
{
    public class ReactionMessageRequest
    {
        public Guid MessageId { get; set; }
        public required string Reaction { get; set; }
    }
}
