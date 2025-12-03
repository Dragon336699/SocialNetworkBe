using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Group
{
    public class GetPendingJoinRequestsResponse
    {
        public required string Message { get; set; }
        public List<GroupUserDto>? PendingRequests { get; set; }
        public int TotalCount { get; set; }
    }
}
