using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests
{
    public class MarkNotiAsReadRequest
    {
        public Guid NotificationId { get; set; }
    }
}
