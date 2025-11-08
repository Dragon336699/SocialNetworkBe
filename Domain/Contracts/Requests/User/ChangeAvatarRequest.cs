using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.User
{
    public class UpdateAvatarRequest
    {
        public IFormFile? Avatar { get; set; }
    }
}
