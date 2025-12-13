using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Search
{
    public class SaveSearchHistoryRequest
    {
        public required string Keyword { get; set; }
        public Guid? SearchedUserId { get; set; }
        public Guid? GroupId { get; set; }
    }
}
