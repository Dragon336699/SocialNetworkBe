using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<IEnumerable<Post>> FindAsyncWithIncludesAndReactionUsers(
            Expression<Func<Post, bool>> predicate,
            params Expression<Func<Post, object>>[] includes);
    }
}
