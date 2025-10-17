using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Message
{
    public class GetMessagesRequest
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public required string ReceiverUserName { get; set; }
        public int Skip {  get; set; }
        public int Take { get; set; }
    }
}
