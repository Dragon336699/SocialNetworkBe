using Domain.Enum.Message.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Message
{
    public class UpdateMessageStatusRequest
    {
        public Guid MessageId { get; set; }
        public MessageStatus Status { get; set; }
    }
}

