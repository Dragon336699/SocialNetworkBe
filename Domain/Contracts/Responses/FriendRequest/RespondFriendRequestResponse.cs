using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.FriendRequest
{
    public class RespondFriendRequestResponse
    {
        public required string Message { get; set; }
        public FriendRequestDto? FriendRequest { get; set; }
    }
}
