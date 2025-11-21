using Domain.Contracts.Responses.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Group
{
    public class GroupDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string ImageUrl { get; set; }
        public bool IsPublic { get; set; }
        public int MemberCount { get; set; }
        public int PostCount { get; set; }
        public List<GroupUserDto>? GroupUsers { get; set; }
        public List<PostDto>? Posts { get; set; }
    }

}
