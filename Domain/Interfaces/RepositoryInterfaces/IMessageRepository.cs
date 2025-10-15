using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<List<Message>?> GetMessages(Guid senderId, Guid receiverId);
    }
}
