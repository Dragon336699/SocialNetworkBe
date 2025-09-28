using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class NotificationUser
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid NotificationId { get; set; }
        public Notification? Notification { get; set; }
        [Required]
        public int IsRead { get; set; }
        [Required]
        public int IsMuted { get; set; }
    }
}
