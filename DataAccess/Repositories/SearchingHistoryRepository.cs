using DataAccess.DbContext;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class SearchingHistoryRepository : GenericRepository<SearchingHistory>, ISearchingHistoryRepository
    {
        public SearchingHistoryRepository(SocialNetworkDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SearchingHistory>?> GetRecentSearchesByUserAsync(Guid userId, int take = 10)
        {
            IEnumerable<SearchingHistory>? histories = await _context.Set<SearchingHistory>()
                .Where(sh => sh.UserId == userId)
                .Include(sh => sh.SearchedUser)
                .Include(sh => sh.Group)
                .AsNoTracking()
                .OrderByDescending(sh => sh.Id)
                .Take(take)
                .ToListAsync();
            return histories;
        }
    }
}