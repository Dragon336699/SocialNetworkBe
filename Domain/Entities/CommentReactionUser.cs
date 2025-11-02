using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CommentReactionUser
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid CommentId { get; set; }
        [JsonIgnore]
        public Comment? Comment { get; set; }
        public required string Reaction { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}
