using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class CommentImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }

        public Comment? Comment { get; set; }
        [Required]
        public required string ImageUrl { get; set; }
    }
}
