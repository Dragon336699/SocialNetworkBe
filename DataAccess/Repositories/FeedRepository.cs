using Cassandra;
using DataAccess.DbContext;
using Domain.Contracts.Requests.Post;
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

        public void FeedForPost(Guid postId, List<Guid> userIds)
        {
            _ = Task.Run(async () =>
            {
                var query = "INSERT INTO user_feed_unseen (user_id, created_at, feed_id, post_id) VALUES (?, ?, ?, ?)";
                var prepared = await _context.Session.PrepareAsync(query);

                foreach (var id in userIds)
                {
                    var createdAtDt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var bound = prepared.Bind(id, createdAtDt, Guid.NewGuid(), postId);
                    await _context.Session.ExecuteAsync(bound);
                }
            });
        }

        public async Task<List<UserFeedUnseen>> GetFeedsForUser(Guid userId)
        {

            var query = "SELECT * FROM user_feed_unseen WHERE user_id = ? LIMIT 10";
            var stmt = _context.Session.Prepare(query);
            var bound = stmt.Bind(userId);

            var rs = await _context.Session.ExecuteAsync(bound);

            // Map RowSet sang class
            var feeds = rs.Select(row =>
            {
                var dt = row.GetValue<DateTimeOffset>("created_at");
                long unixTime = dt.ToUnixTimeMilliseconds();
                return new UserFeedUnseen
                {
                    UserId = row.GetValue<Guid>("user_id"),
                    CreatedAt = unixTime,
                    PostId = row.GetValue<Guid>("post_id"),
                    FeedId = row.GetValue<Guid>("feed_id"),
                };
            }).ToList();

            return feeds;
        }

        public void SeenFeed(List<SeenFeedRequest> request, Guid userId)
        {
            var deleteCommand = "DELETE FROM user_feed_unseen WHERE user_id = ? AND feed_id = ? AND created_at = ?";
            var query = "INSERT INTO user_feed_seen (user_id, post_id, feed_id, seen_at) VALUES (?, ?, ?, ?)";
            var deletePrepared = _context.Session.Prepare(deleteCommand);
            var prepared = _context.Session.Prepare(query);
            var batch = new BatchStatement();
            batch.SetBatchType(BatchType.Unlogged);

            foreach ( var feed in request)
            {
                var createdAtDt = DateTimeOffset.FromUnixTimeMilliseconds(feed.CreatedAt).UtcDateTime;
                var seenAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                batch.Add(deletePrepared.Bind(userId, feed.FeedId, createdAtDt));
                batch.Add(prepared.Bind(userId, feed.PostId, feed.FeedId, seenAt));
            }
            _context.Session.Execute(batch);
        }
    }
}
