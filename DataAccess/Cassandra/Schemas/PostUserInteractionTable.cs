using Cassandra;

namespace DataAccess.Cassandra.Schemas
{
    public class PostUserInteractionTable : ICassandraSchema
    {
        public async Task CreateAsync(ISession session)
        {
            await session.ExecuteAsync(new SimpleStatement(
                @"CREATE TABLE IF NOT EXISTS post_user_interaction (
                post_id UUID,
                user_id UUID,
                action TEXT,
                created_at TIMESTAMP,
                PRIMARY KEY ((post_id), user_id, action)
                );"
             ));
        }
    }
}
