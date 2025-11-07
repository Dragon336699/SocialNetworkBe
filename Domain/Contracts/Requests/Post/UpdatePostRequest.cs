using Domain.Enum.Post.Types;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Post
{
    public class UpdatePostRequest
    {
        public string? Content { get; set; }
        public PostPrivacy? PostPrivacy { get; set; }
        public List<IFormFile>? NewImages { get; set; }
        public List<Guid>? ImageIdsToDelete { get; set; }
        public bool RemoveAllImages { get; set; } = false;
    }
}
