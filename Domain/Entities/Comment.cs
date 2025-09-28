using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required string Content { get; set; }
        public Guid? RepliedCommentId { get; set; }
        [ForeignKey(nameof(RepliedCommentId))]
        public Comment? ParentComment { get; set; }
        public ICollection<CommentImage>? CommentImage { get; set; }
    }
}
