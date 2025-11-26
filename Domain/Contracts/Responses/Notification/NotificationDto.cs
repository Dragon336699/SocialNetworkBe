using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Notification
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public List<string>? ImageUrls { get; set; }
        public string? NavigateUrl { get; set; }
        public bool Unread { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid ReceiverId { get; set; }
        public List<HighlightOffset>? Highlights { get; set; }
    }
}
