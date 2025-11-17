using Domain.Contracts.Responses.Notification;
using Domain.Enum.Notification.Types;
using Microsoft.EntityFrameworkCore;
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
        public required NotificationData Data { get; set; }
        [Required]
        public required string MergeKey { get; set; } // type + TargetId + userId ( người nhận )
        public string? NavigateUrl { get; set; }
        [Required]
        public bool Unread { get; set; } = true;
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public Guid ReceiverId { get; set; }
        public User? Receiver { get; set; }
    }
}
