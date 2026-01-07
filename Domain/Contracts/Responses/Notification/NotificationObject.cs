using Domain.Enum.Notification.Types;

namespace Domain.Contracts.Responses.Notification
{
    public class NotificationObject
    {
        public Guid? Id { get; set; }
        public string? Name {  get; set; }
        public required NotificationObjectType Type { get; set; }
        public string? SnapshotText { get; set; }
    }
}
