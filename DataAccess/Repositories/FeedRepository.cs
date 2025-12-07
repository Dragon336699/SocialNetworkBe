using DataAccess.DbContext;
using Domain.Contracts.Responses.Post.UserFeed;
using Domain.Interfaces.RepositoryInterfaces;
using Cassandra.Mapping;

namespace DataAccess.Repositories
{
    public class FeedRepository : IFeedRepository
    {
        private readonly CassandraContext _context;

        public FeedRepository(CassandraContext context)
        {
            _context = context;
        }

        public async void FeedForPost(Guid postId, List<Guid> userIds, Guid authorId)
        {
            var initSeenAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var query = "INSERT INTO user_feed (user_id, post_id, author_id, created_at, seen_at) VALUES (?, ?, ?, ?, ?)";
            var prepared = await _context.Session.PrepareAsync(query);

            foreach (var id in userIds)
            {
                var bound = prepared.Bind(id, postId, authorId, DateTime.UtcNow, initSeenAt);
                await _context.Session.ExecuteAsync(bound);
            }
        }

        public async Task<List<UserFeedResponse>> GetFeedsForUser(Guid userId)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var query = "SELECT * FROM user_feed WHERE user_id = ? AND seen_at = ? LIMIT 10 ALLOW FILTERING";
            var stmt = _context.Session.Prepare(query);
            var bound = stmt.Bind(userId, epoch);

            var rs = await _context.Session.ExecuteAsync(bound);

            // Map RowSet sang class
            var feeds = rs.Select(row => new UserFeedResponse
            {
                UserId = row.GetValue<Guid>("user_id"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                AuthorId = row.GetValue<Guid>("author_id"),
                PostId = row.GetValue<Guid>("post_id"),
                SeenAt = row.GetValue<DateTime>("seen_at")
            }).ToList();

            return feeds;
        }
    }
}
