using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Notification
{
    public class NotificationData
    {
        public required List<NotificationObject> Subjects { get; set; }
        public int SubjectCount { get; set; }
        public required string Content { get; set; }
        public required NotificationObject DiObject {  get; set; } // Direct Object
        public NotificationObject? InObject { get; set; } // Indirect Object
        public NotificationObject? PrObject { get; set; } // Prepositional Object
        public List<HighlightOffset>? Highlights { get; set; }
    }
}
