using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Reaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string IconSymbol { get; set; }
        public ICollection<PostReactionUser>? PostReactionUsers { get; set; }
        public ICollection<MessageReactionUser>? MessageReactionUsers { get; set; }
        public ICollection<CommentReactionUser>? CommentReactionUsers { get; set; }
    }
}
