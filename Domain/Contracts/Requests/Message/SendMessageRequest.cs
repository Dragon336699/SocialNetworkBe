using Domain.Enum.MessageAttachment.Types;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Message
{
    public class SendMessageRequest
    {
        public Guid SenderId { get; set; }
        public Guid ConversationId { get; set; }
        public Guid? RepliedMessageId { get; set; }
        public string? Content { get; set; }
        public List<IFormFile>? Files { get; set; }
        public FileTypes? FileType { get; set; }
    }
}
