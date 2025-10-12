using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SocialNetworkBe.SignalR
{
    public class CustomerUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
