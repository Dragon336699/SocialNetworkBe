using Domain.Interfaces.ServiceInterfaces;

namespace SocialNetworkBe.Services.ScheduleServices
{
    public class FeedScheduleService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;
        public FeedScheduleService(IHttpClientFactory factory, IServiceProvider serviceProvider)
        {
            _client = factory.CreateClient("FeedPostSuggest");
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async _ =>
            {
                await RunFeedJob();
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));

            return Task.CompletedTask;
        }

        private async Task RunFeedJob()
        {
            using var scope = _serviceProvider.CreateScope();
                
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            var activeUsers = await userService.GetActiveUsers();

            if (activeUsers == null) return;

            await Parallel.ForEachAsync(activeUsers, new ParallelOptions { MaxDegreeOfParallelism = 20 },
                async (userId, ct) =>
                {
                    var response = await _client.PostAsync($"ai/postSuggest?user_id={userId}", null, ct);
                    response.EnsureSuccessStatusCode();
                });
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
