using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Group
{
    public class GroupInvitationDto
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string GroupDescription { get; set; } = string.Empty;
        public string GroupImageUrl { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public int MemberCount { get; set; }
        public DateTime InvitedAt { get; set; }
    }
}
