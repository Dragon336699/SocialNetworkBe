using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Group
{
    public class KickMemberResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
