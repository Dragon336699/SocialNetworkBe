using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Post
{
    public class CreatePostRequest
    {
        [Required]
        [StringLength(5000, MinimumLength = 1)]
        public required string Content { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}
