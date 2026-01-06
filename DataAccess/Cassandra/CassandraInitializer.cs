using Cassandra;

namespace DataAccess.Cassandra
{
    public class CassandraInitializer
    {
        private readonly IEnumerable<ICassandraSchema> _schemas;
        public CassandraInitializer(IEnumerable<ICassandraSchema> schemas)
        {
            _schemas = schemas;
        }

        public async Task InitializeAsync(ISession session)
        {
            foreach (var schema in _schemas)
            {
                await schema.CreateAsync(session);
            }
        }
    }
}
