using Cassandra;

namespace DataAccess.Cassandra.Schemas
{
    public class UserFeedUnseenTable : ICassandraSchema
    {
        public async Task CreateAsync(ISession session)
        {
            await session.ExecuteAsync(new SimpleStatement(
                @"CREATE TABLE IF NOT EXISTS user_feed_unseen (
                 user_id UUID,
                post_id UUID,
                feed_id UUID,
                created_at TIMESTAMP,
                PRIMARY KEY ((user_id), created_at, feed_id)
                ) WITH CLUSTERING ORDER BY (created_at DESC, feed_id ASC);"
             ));
        }
    }
}
