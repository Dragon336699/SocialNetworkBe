using Cassandra;

namespace DataAccess.Cassandra.Schemas
{
    public class UserPostInteractionTable : ICassandraSchema
    {
        public async Task CreateAsync(ISession session)
        {
            await session.ExecuteAsync(new SimpleStatement(
                @"CREATE TABLE IF NOT EXISTS user_post_interaction (
                user_id UUID,
                post_id UUID,
                action TEXT,
                created_at TIMESTAMP,
                PRIMARY KEY ((user_id), created_at, post_id)
                ) WITH CLUSTERING ORDER BY (created_at DESC);"
             ));
        }
    }
}
