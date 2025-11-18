using Domain.Contracts.Responses.User;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Comment
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalLiked { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public Guid? RepliedCommentId { get; set; }
        public CommentDto? ParentComment { get; set; }
        public List<CommentDto>? Replies { get; set; }
        public List<CommentImageDto>? CommentImages { get; set; }
        public ICollection<CommentReactionUser>? CommentReactionUsers { get; set; }
    }
}
