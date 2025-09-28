using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class FriendRequest
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        [Required]
        public required string FriendRequestStatus { get; set; }
        public User? Sender { get; set; }
        public User? Receiver { get; set; }
    }
}
