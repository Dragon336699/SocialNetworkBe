using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.FriendRequest
{
    public class CancelFriendRequestRequest
    {
        public Guid ReceiverId { get; set; }
    }
}
