using Domain.Entities;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.SignalR;
using SocialNetworkBe.ChatServer;

namespace SocialNetworkBe.Services.RealtimeServices
{
    public class RealTimeService : IRealtimeService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public RealTimeService(
            IHubContext<ChatHub> hubContext
        )
        {
            _hubContext = hubContext; 
        }

        public async Task SendPrivateNotification(Notification noti, Guid receiverId)
        {
            await _hubContext.Clients.User(receiverId.ToString().ToLower()).SendAsync("SendPrivateNoti", noti);
        }
    }
}
