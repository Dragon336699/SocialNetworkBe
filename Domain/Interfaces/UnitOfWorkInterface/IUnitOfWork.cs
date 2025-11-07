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
        IMessageReactionUserRepository MessageReactionUserRepository { get; }

        IPostRepository PostRepository { get; }
        IUserRepository UserRepository { get; }

        IMessageAttachmentRepository MessageAttachmentRepository { get; }
        IPostReactionUserRepository PostReactionUserRepository { get; }
        IFriendRequestRepository FriendRequestRepository { get; }
        IUserRelationRepository UserRelationRepository { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
