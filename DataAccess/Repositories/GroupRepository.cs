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
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(SocialNetworkDbContext context) : base(context)
        {
        }

        public async Task<List<Group>> GetGroupsWithFullDetailsAsync(
            Expression<Func<Group, bool>> predicate,
            int skip = 0,
            int take = int.MaxValue)
        {
            return await _context.Set<Group>()
                .AsNoTracking()
                .AsSplitQuery()
                .Where(predicate)
                .OrderBy(g => g.Id)
                .Include(g => g.GroupUsers)
                    .ThenInclude(gu => gu.User)
                .Include(g => g.Posts)
                    .ThenInclude(p => p.User)
                .Include(g => g.Posts)
                    .ThenInclude(p => p.PostImages)
                .Include(g => g.Posts)
                    .ThenInclude(p => p.PostReactionUsers)
                        .ThenInclude(pru => pru.User)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Group?> GetGroupWithFullDetailsByIdAsync(Guid groupId)
        {
            return await _context.Set<Group>()
                .AsNoTracking()
                .AsSplitQuery()
                .Where(g => g.Id == groupId)
                .Include(g => g.GroupUsers)
                    .ThenInclude(gu => gu.User)
                .Include(g => g.Posts)
                    .ThenInclude(p => p.User)
                .Include(g => g.Posts)
                    .ThenInclude(p => p.PostImages)
                .Include(g => g.Posts)
                    .ThenInclude(p => p.PostReactionUsers)
                        .ThenInclude(pru => pru.User)
                .FirstOrDefaultAsync();
        }
    }

}
