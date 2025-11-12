using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Comment
{
    public class GetCommentsResponse
    {
        public required string Message { get; set; }
        public List<CommentDto>? Comments { get; set; }
        public int TotalCount { get; set; }
    }
}
