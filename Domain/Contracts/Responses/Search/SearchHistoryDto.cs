using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Search
{
    public class SearchHistoryDto
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? NavigateUrl { get; set; }
    }
}
