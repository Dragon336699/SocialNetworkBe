using Domain.Contracts.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Group
{
    public class GroupUserDto
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public string RoleName { get; set; }
        public DateTime JoinedAt { get; set; }
        public UserDto? User { get; set; }

    }
}
