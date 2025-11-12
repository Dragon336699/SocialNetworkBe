using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.FriendRequest
{
    public class GetFriendRequestsResponse
    {
        public required string Message { get; set; }
        public List<FriendRequestDto>? FriendRequests { get; set; }
        public int TotalCount { get; set; }
    }
}
