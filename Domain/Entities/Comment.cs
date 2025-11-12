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
        //[Required]
        //public DateTime CreatedAt { get; set; }
        //[Required]
        //public DateTime UpdatedAt { get; set; }
        //public int TotalLiked { get; set; } = 0;
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid? RepliedCommentId { get; set; }
        [ForeignKey(nameof(RepliedCommentId))]
        public Comment? ParentComment { get; set; }
        public ICollection<Comment>? Replies { get; set; }
        public ICollection<CommentImage>? CommentImage { get; set; }
        public ICollection<CommentReactionUser>? CommentReactionUsers { get; set; }     

    }
}
