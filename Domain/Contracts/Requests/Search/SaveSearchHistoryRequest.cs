using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Search
{
    public class SaveSearchHistoryRequest
    {
        public required string Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? NavigateUrl { get; set; }
    }
}
