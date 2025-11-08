using Domain.Enum.FriendRequest.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.FriendRequest
{
    public class RespondFriendRequestRequest
    {
        public Guid SenderId { get; set; }
        public FriendRequestStatus Status { get; set; } 
    }
}
