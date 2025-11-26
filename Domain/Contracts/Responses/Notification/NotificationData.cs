using Domain.Enum.Notification.Types;

namespace Domain.Contracts.Responses.Notification
{
    public class NotificationData
    {
        public required List<NotificationObject> Subjects { get; set; }
        public int SubjectCount { get; set; }
        public required Verb Verb { get; set; }
        public required NotificationObject DiObject { get; set; } // Direct Object
        public NotificationObject? InObject { get; set; } // Indirect Object
        public NotificationObject? PrObject { get; set; } // Prepositional Object
        public Preposition? Preposition { get; set; }
    }
}
