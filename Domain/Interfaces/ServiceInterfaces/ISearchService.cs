using Domain.Contracts.Requests.Search;
using Domain.Contracts.Responses.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface ISearchService
    {
        Task<SearchResultDto?> SearchAsync(SearchRequest request, Guid userId, bool saveHistory = true);
        Task<bool> SaveSearchHistoryAsync(Guid userId, string keyword, Guid? searchedUserId = null, Guid? groupId = null);
        Task<List<SearchHistoryDto>?> GetRecentSearchesAsync(Guid userId, int take = 10);
        Task<bool> DeleteSearchHistoryAsync(Guid userId, Guid historyId);
        Task<bool> ClearAllSearchHistoryAsync(Guid userId);
    }
}
