using Domain.Contracts.Responses.User;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Message
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }                
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid SenderId { get; set; }
        public Guid? RepliedMessageId { get; set; }
        public MessageDto? RepliedMessage { get; set; }
        public List<MessageAttachment>? MessageAttachments { get; set; }
        public required UserDto Sender { get; set; }

    }
}
