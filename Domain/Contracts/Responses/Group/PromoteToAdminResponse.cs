using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Group
{
    public class PromoteToAdminResponse
    {
        public string Message { get; set; }
        public GroupUserDto? GroupUser { get; set; }
    }
}
