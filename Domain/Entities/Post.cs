using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required string Content { get; set; }
        [Required]
        public int TotalLiked { get; set; }
        [Required]
        public int TotalComment {  get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<PostImage>? PostImages { get; set; }
        public ICollection<PostReactionUser>? PostReactionUsers { get; set; }

        //Liên kết đến User
        public Guid? UserId { get; set; }  
        public User? User { get; set; }    
    }
}
