using Cassandra;

namespace DataAccess.Cassandra
{
    public interface ICassandraSchema
    {
        Task CreateAsync(ISession session);
    }
}
