using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Post
{
    public class GetPendingPostsResponse
    {
        public required string Message { get; set; }
        public List<PostDto>? Posts { get; set; }
        public int TotalCount { get; set; }
    }
}
