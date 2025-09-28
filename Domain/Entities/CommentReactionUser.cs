using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CommentReactionUser
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid CommentId { get; set; }
        public Comment? Post { get; set; }
        public Guid ReactionId { get; set; }
        public Reaction? Reaction { get; set; }
    }
}
