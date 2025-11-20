using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<List<Comment>?> GetCommentsByPostIdAsync(Guid postId, int skip = 0, int take = 10);
        Task<List<Comment>?> GetRepliesByCommentIdAsync(Guid commentId, int skip = 0, int take = 10);
        Task<Comment?> GetCommentByIdWithTrackingAsync(Guid commentId);
    }
}
