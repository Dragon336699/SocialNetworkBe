using Cassandra;

public class CassandraContext : IDisposable
{
    private Cluster? _cluster;
    private ISession? _session;
    private bool _initialized;

    public ISession Session
    {
        get
        {
            ConnectToCass();
            return _session!;
        }
    }

    private void ConnectToCass()
    {
        if (_initialized) return;

        _cluster = Cluster.Builder()
            .AddContactPoint("127.0.0.1")
            .WithPort(9042)
            .Build();

        _session = _cluster.Connect();

        // Create keyspace if not exists
        _session.Execute(@"
            CREATE KEYSPACE IF NOT EXISTS fricon 
            WITH replication = {
                'class': 'SimpleStrategy', 
                'replication_factor': 1
            };
        ");

        _session.ChangeKeyspace("fricon");

        _initialized = true;
    }

    public void Dispose()
    {
        _session?.Dispose();
        _cluster?.Dispose();
    }
}
