using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IRealtimeService
    {
        Task SendPrivateNotification(Notification noti, Guid receiverId);
    }
}
