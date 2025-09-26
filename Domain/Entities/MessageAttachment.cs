using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MessageAttachment
    {
        [Key]
        public Guid MessageId { get; set; }
        [Required]
        public required string FileUrl { get; set; }
        public Message? Message { get; set; }
    }
}
