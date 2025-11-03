using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Post
{
    public class PostImageDto
    {
        public Guid Id { get; set; }
        public required string ImageUrl { get; set; }
    }
}
