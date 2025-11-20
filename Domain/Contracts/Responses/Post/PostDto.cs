using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Post.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Post
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public int TotalLiked { get; set; }
        public int TotalComment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PostPrivacy PostPrivacy { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public Guid? GroupId { get; set; }
        public List<PostImageDto>? PostImages { get; set; }
        public ICollection<PostReactionUserDto>? PostReactionUsers { get; set; }
    }
}
