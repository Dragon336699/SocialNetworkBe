using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class MessageReactionUser
    {
        public Guid UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        public Guid MessageId { get; set; }
        [JsonIgnore]
        public Message? Message { get; set; }
        public required string Reaction { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}
