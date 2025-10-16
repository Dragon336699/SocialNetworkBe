using Domain.Interfaces.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.UnitOfWorkInterface
{
    public interface IUnitOfWork : IDisposable
    {
        IMessageRepository MessageRepository { get; }
        IConversationUserRepository ConversationUserRepository { get; }
        IConversationRepository ConversationRepository { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
