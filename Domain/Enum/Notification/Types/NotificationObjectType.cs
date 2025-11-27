using System.Text.Json.Serialization;

namespace Domain.Enum.Notification.Types
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NotificationObjectType
    {
        Actor,
        Group,
        Post,
        FriendRequest,
        Comment
    }
}
