using Domain.Contracts.Requests.Comment;
using Domain.Contracts.Responses.Comment;
using Domain.Enum.Comment.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface ICommentService
    {
        Task<(CreateCommentEnum, Guid?)> CreateCommentAsync(CreateCommentRequest request, Guid userId);
        Task<(GetCommentsEnum, List<CommentDto>?)> GetCommentsByPostIdAsync(Guid postId, int skip = 0, int take = 10);
        Task<(UpdateCommentEnum, CommentDto?)> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request, Guid userId);
        Task<(DeleteCommentEnum, bool)> DeleteCommentAsync(Guid commentId, Guid userId);
        Task<CommentDto?> AddUpdateDeleteReactionComment(ReactionCommentRequest request, Guid userId);
    }
}
