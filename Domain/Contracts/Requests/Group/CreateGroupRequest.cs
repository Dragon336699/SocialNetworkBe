using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Group
{
    public class CreateGroupRequest
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsPublic { get; set; } = true;
        public IFormFile? Image { get; set; }
    }

}
