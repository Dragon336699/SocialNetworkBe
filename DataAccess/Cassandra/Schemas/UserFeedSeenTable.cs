using Cassandra;

namespace DataAccess.Cassandra.Schemas
{
    public class UserFeedSeenTable : ICassandraSchema
    {
        public async Task CreateAsync(ISession session)
        {
            await session.ExecuteAsync(new SimpleStatement(
                @"CREATE TABLE IF NOT EXISTS user_feed_seen (
                 user_id UUID,
                post_id UUID,
                feed_id UUID,
                seen_at TIMESTAMP,
                PRIMARY KEY ((user_id), feed_id)
                );"
             ));
        }
    }
}
