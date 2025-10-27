using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Post
{
    public class CreatePostResponse
    {
        public Guid? PostId { get; set; }      
        public required string Message { get; set; } 
    }
}
