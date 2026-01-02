using Cassandra;

namespace DataAccess.Cassandra.Schemas
{
    public class UserInteractionCounterTable : ICassandraSchema
    {
        public async Task CreateAsync(ISession session)
        {
            await session.ExecuteAsync(new SimpleStatement(
                @"CREATE TABLE IF NOT EXISTS user_interaction_counter (
                user_id UUID,
                target_user_id UUID,
                view_count COUNTER,
                like_count COUNTER,
                search_count COUNTER,
                PRIMARY KEY ((user_id), target_user_id)
                );"
             ));
        }
    }
}
