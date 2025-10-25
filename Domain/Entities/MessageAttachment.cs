using Domain.Enum.MessageAttachment.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class MessageAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        [Required]
        public required string FileUrl { get; set; }
        [Required]
        public FileTypes FileType { get; set; } 
        [JsonIgnore]
        public Message? Message { get; set; }
    }
}
