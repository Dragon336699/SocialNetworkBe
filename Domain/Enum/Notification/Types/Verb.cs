using System.Text.Json.Serialization;

namespace Domain.Enum.Notification.Types
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Verb
    {
        Liked,
        Commented,
        Sent,
        Invited,
        Replied
    }
}
