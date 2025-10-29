using Domain.Contracts.Requests.ConversationUser;
using Domain.Entities;
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
        Task AddUsersToConversationAsync(Guid conversationId, List<Guid> userIds);
        Task<IEnumerable<ConversationUser>?> GetConversationUser(GetConversationUserRequest request);
        Task<IEnumerable<ConversationUser>?> GetConversationUsersByUserId(Guid userId);
    }
}
