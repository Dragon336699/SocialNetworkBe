using Cassandra;

namespace DataAccess.Cassandra.Schemas
{
    public class UserInteractionMetaTable : ICassandraSchema
    {
        public async Task CreateAsync(ISession session)
        {
            await session.ExecuteAsync(new SimpleStatement(
                @"CREATE TABLE IF NOT EXISTS user_interaction_meta (
                user_id UUID,
                target_user_id UUID,
                last_interaction TIMESTAMP,
                PRIMARY KEY ((user_id), target_user_id)
                );"
             ));
        }
    }
}
