namespace Domain.Contracts.Responses.Notification
{
    public class NotificationObject
    {
        public Guid Id { get; set; }
        public required string Name {  get; set; }
        public required string Type { get; set; }
        public string? ImageUrl { get; set; } = null;
    }
}
