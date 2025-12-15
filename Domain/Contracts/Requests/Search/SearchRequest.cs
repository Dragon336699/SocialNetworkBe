using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum.Search.Types;
namespace Domain.Contracts.Requests.Search
{
    public class SearchRequest
    {
        public required string Keyword { get; set; }
        public SearchType? Type { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }

}
