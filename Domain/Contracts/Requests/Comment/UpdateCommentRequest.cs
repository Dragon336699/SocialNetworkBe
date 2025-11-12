using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Comment
{
    public class UpdateCommentRequest
    {
        public string? Content { get; set; }
        public List<IFormFile>? NewImages { get; set; }
        public List<Guid>? ImageIdsToDelete { get; set; }
    }
}
