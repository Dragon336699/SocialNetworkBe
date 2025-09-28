using Domain.Enum.Conversation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public ConversationType Type { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public ICollection<ConversationUser>? ConversationUsers { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
