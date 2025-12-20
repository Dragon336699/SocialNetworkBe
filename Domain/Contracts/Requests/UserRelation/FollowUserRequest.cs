using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.UserRelation
{
    public class UserIdRequest
    {
        public Guid TargetUserId { get; set; }
    }
}
