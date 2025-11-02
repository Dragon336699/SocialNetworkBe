using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class PostReactionUser
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid PostId { get; set; }
        [JsonIgnore]
        public Post? Post { get; set; }
        public required string Reaction { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}
