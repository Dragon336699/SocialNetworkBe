using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Comment
{
    public class CreateCommentRequest
    {
        public required string Content { get; set; }
        public Guid PostId { get; set; }
        public Guid? RepliedCommentId { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
