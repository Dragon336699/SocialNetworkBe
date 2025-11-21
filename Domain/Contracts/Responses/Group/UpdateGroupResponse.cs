using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Group
{
    public class UpdateGroupResponse
    {
        public required string Message { get; set; }
        public GroupDto? Group { get; set; }
    }

}
