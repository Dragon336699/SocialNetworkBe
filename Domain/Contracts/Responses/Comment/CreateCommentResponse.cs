using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Comment
{
    public class CreateCommentResponse
    {
        public Guid? CommentId { get; set; }
        public required string Message { get; set; }
    }
}
