using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IConversationUserService
    {
        Task<Guid?> CheckExist(Guid senderId, Guid receiverId);
        Task AddUsersToConversationAsync(Guid conversationId, Guid senderId, Guid receiverId);
    }
}
