using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        Task<Conversation?> GetConversationBetweenUsersAsync(Guid user1Id, Guid user2Id);
    }
}
