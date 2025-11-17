using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Comment
{
    public class UpdateCommentResponse
    {
        public required string Message { get; set; }
        public CommentDto? Comment { get; set; }
    }
}
