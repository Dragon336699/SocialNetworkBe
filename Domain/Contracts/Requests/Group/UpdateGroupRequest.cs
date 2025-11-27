using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Group
{
    public class UpdateGroupRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsPublic { get; set; }
        public IFormFile? NewImage { get; set; }
        public bool RemoveImage { get; set; } = false;
    }

}
