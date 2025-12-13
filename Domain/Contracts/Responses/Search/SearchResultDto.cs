using Domain.Contracts.Responses.Group;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Search
{
    public class SearchResultDto
    {
        public List<UserDto>? Users { get; set; }
        public List<GroupDto>? Groups { get; set; }
        public List<PostDto>? Posts { get; set; }
        public int TotalUsersCount { get; set; }
        public int TotalGroupsCount { get; set; }
        public int TotalPostsCount { get; set; }
    }
}
