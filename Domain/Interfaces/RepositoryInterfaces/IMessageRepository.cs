using Domain.Contracts.Responses.Message;
using Domain.Entities;
using Domain.Enum.Message.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<List<Message>?> GetMessages(Guid senderId, Guid receiverId, int skip, int take);
        Task<Message?> UpdateAllMessagesStatus(Guid messageId, MessageStatus messageStatus);
    }
}
