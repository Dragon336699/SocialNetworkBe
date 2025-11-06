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
        public required Guid ConversationId {  get; set; }
        public Guid SenderId { get; set; }
        public Guid? RepliedMessageId { get; set; }
        public MessageDto? RepliedMessage { get; set; }
        public ICollection<MessageReactionUser>? MessageReactionUsers { get; set; }
        public List<MessageAttachment>? MessageAttachments { get; set; }
        public UserDto? Sender { get; set; }

    }
}
