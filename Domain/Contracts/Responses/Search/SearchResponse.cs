using Domain.Contracts.Responses.Group;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.Search
{
    public class SearchResponse
    {
        public required string Message { get; set; }
        public SearchResultDto? Results { get; set; }
    }
}
