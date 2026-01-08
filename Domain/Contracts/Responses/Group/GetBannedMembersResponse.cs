using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Group
{
    public class GetBannedMembersResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<GroupUserDto>? BannedMembers { get; set; }
        public int TotalCount { get; set; }
    }
}
