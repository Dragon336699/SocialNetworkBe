using Domain.Contracts.Responses.Post;
using Domain.Entities.NoSQL;

namespace Domain.Contracts.Responses.Feed
{
    public class FeedDto
    {
        public long CreatedAt { get; set; }
        public required PostDto Post { get; set; }
    }
}
