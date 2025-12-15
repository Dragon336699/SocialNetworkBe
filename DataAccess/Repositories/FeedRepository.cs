using DataAccess.DbContext;
using Domain.Entities.NoSQL;
using Domain.Interfaces.RepositoryInterfaces;

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

            var query = "INSERT INTO user_feed_unseen (user_id, created_at, feed_id, post_id) VALUES (?, ?, ?, ?)";
            var prepared = await _context.Session.PrepareAsync(query);

            foreach (var id in userIds)
            {
                var bound = prepared.Bind(id, DateTime.UtcNow, Guid.NewGuid(), postId);
                await _context.Session.ExecuteAsync(bound);
            }
        }

        public async Task<List<UserFeedUnseen>> GetFeedsForUser(Guid userId)
        {

            var query = "SELECT * FROM user_feed_unseen WHERE user_id = ? LIMIT 10";
            var stmt = _context.Session.Prepare(query);
            var bound = stmt.Bind(userId);

            var rs = await _context.Session.ExecuteAsync(bound);

            // Map RowSet sang class
            var feeds = rs.Select(row => new UserFeedUnseen
            {
                UserId = row.GetValue<Guid>("user_id"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                PostId = row.GetValue<Guid>("post_id"),
                FeedId = row.GetValue<Guid>("feed_id"),
            }).ToList();

            return feeds;
        }
    }
}
