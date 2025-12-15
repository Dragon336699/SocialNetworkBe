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
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        [AllowNull]
        public string? ImageUrl { get; set; }
        [AllowNull]
        public string? NavigateUrl { get; set; }
    }
}
