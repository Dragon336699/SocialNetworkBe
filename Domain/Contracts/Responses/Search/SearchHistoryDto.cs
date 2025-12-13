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
        public Guid? SearchedUserId { get; set; }
        public string? SearchedUserName { get; set; }
        public string? SearchedUserAvatar { get; set; }
        public Guid? GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? GroupImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
