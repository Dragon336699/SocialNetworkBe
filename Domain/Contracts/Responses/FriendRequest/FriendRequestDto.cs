using Domain.Contracts.Responses.User;
using Domain.Enum.FriendRequest.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.FriendRequest
{
    public class FriendRequestDto
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public FriendRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Message { get; set; }
        public UserDto? Sender { get; set; }
        public UserDto? Receiver { get; set; }
    }
}
