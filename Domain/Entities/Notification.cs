using Domain.Enum.Notification;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required NotificationType NoficationType { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public required string Content { get; set; }

    }
}
