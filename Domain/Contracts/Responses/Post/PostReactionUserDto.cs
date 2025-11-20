using Domain.Contracts.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Post
{
    public class PostReactionUserDto
    {
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public Guid PostId { get; set; }
        public required string Reaction { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}
