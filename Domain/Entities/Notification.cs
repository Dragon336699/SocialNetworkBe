using Domain.Enum.Notification.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public ICollection<NotificationUser>? NotificationUsers { get; set; }

    }
}
