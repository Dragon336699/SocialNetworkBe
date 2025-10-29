using Domain.Interfaces.ChatInterfaces;
using System.Collections.Concurrent;

namespace SocialNetworkBe.SignalR
{
    public class UserConnectionManager : IUserConnectionManager
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> _connections = new ConcurrentDictionary<string, HashSet<string>>();
        public Task AddConnectionAsync (string userId, string connectionId)
        {
            var connections = _connections.GetOrAdd(userId, _ => new HashSet<string>());
            lock (_connections)
            {
                connections.Add(connectionId);
            }
            return Task.CompletedTask;
        }

        public Task RemoveConnectionAsync (string userId, string connectionId)
        {
            if (_connections.TryGetValue(userId, out var connections))
            {
                lock (connections)
                {
                    connections.Remove(connectionId);
                }
                if (connections.Count == 0) _connections.TryRemove(userId, out _);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>?> GetConnectionAsync (Guid userId)
        {
            string userIdString = userId.ToString();
            if (_connections.TryGetValue (userIdString, out var connections))
            {
                return Task.FromResult<IEnumerable<string>?>(connections);
            }
            return Task.FromResult<IEnumerable<string>?>(null);
        }
    }
}
