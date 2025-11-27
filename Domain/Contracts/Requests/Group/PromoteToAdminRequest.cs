using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Group
{
    public class PromoteToAdminRequest
    {
        public Guid TargetUserId { get; set; }
    }
}
