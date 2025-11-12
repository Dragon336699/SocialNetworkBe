using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Comment
{
    public class ReactionCommentRequest
    {
        public Guid CommentId { get; set; }
        public required string Reaction { get; set; }
    }
}
