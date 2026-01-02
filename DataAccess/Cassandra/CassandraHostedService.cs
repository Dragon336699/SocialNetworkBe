using Microsoft.Extensions.Hosting;

namespace DataAccess.Cassandra
{
    public class CassandraHostedService : IHostedService
    {
        private readonly CassandraContext _context;
        private readonly CassandraInitializer _initializer;

        public CassandraHostedService(
            CassandraContext context,
            CassandraInitializer initializer)
        {
            _context = context;
            _initializer = initializer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // chạy nền, không block startup
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            try
            {
                await _initializer.InitializeAsync(_context.Session);
            }
            catch (Exception ex)
            {
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
