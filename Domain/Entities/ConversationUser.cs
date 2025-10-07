using Domain.Enum.Conversation.Types;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class ConversationUser
    {
        public Guid UserId { get; set; }
        public Guid ConversationId { get; set; }
        [Required]
        public DateTime JoinedAt { get; set; }
        public string? NickName { get; set; }
        [Required]
        public ConversationRole RoleName { get; set; }
        public string? DraftMessage { get; set; }
        public User? User { get; set; }
        public Conversation? Conversation { get; set; }
    }
}
