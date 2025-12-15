using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface ISearchingHistoryRepository : IGenericRepository<SearchingHistory>
    {
        Task<IEnumerable<SearchingHistory>?> GetRecentSearchesByUserAsync(Guid userId, int take = 10);
    }
}
