using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entities
{
    public class SearchingHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string? Content { get; set; }
        [AllowNull]
        public Guid? SearchedUserId { get; set; }
        public User? SearchedUser { get; set; }
        [AllowNull]
        public Guid? GroupId { get; set; }
        public Group? Group { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
    }
}
