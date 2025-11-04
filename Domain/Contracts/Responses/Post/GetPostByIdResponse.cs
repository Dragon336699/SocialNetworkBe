using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Post
{
    public class GetPostByIdResponse
    {
        public required string Message { get; set; }
        public PostDto? Post { get; set; }
    }
}
