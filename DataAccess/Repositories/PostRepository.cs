using DataAccess.DbContext;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(SocialNetworkDbContext context) : base(context)
        {
        }

        // Override method FindAsyncWithIncludes để include thêm User trong PostReactionUsers
        public async Task<IEnumerable<Post>> FindAsyncWithIncludesAndReactionUsers(
            Expression<Func<Post, bool>> predicate,
            params Expression<Func<Post, object>>[] includes)
        {
            IQueryable<Post> query = _context.Set<Post>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
           
            query = query.Include(p => p.PostReactionUsers)
                        .ThenInclude(pr => pr.User);
       
            return await query.Where(predicate).ToListAsync();
        }

    }
}
