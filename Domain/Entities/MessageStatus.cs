using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MessageStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required MessageStatus Status { get; set; }
        [Required]
        public Guid MessageId { get; set; }
        [Required]
        public Guid ReceiverId { get; set; }
        public Message? Message { get; set; }
        public User? User { get; set; }
    }
}
