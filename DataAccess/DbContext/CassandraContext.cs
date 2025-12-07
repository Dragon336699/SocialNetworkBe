using Cassandra;

namespace DataAccess.DbContext
{
    public class CassandraContext : IDisposable
    {
        private readonly Cluster _cluster;
        public ISession Session { get; }
        public CassandraContext()
        {
            _cluster = Cluster.Builder()
                .AddContactPoint("127.0.0.1")
                .Build();
            // Connect session
            Session = _cluster.Connect();

            // Tạo keyspace nếu chưa có
            Session.Execute(@"CREATE KEYSPACE IF NOT EXISTS timeline WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};");

            // Chọn keyspace
            Session.Execute("USE timeline;");
        }

        public void Dispose()
        {
            Session?.Dispose();
            _cluster?.Dispose();
        }
    }
}
