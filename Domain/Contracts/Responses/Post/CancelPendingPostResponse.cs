using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Post
{
    public class CancelPendingPostResponse
    {
        public required string Message { get; set; }
    }
}
