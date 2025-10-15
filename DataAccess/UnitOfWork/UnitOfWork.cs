using DataAccess.DbContext;
using DataAccess.Repositories;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SocialNetworkDbContext _context;
        public IMessageRepository MessageRepository { get; set; }
        public IConversationUserRepository ConversationUserRepository { get; set; }
        public IConversationRepository ConversationRepository { get; set; }
        public UnitOfWork(
            SocialNetworkDbContext context,
            IMessageRepository messageRepository,
            IConversationUserRepository conversationUserRepository,
            IConversationRepository conversationRepository
        )
        {
            _context = context;
            MessageRepository = messageRepository;
            ConversationUserRepository = conversationUserRepository;
            ConversationRepository = conversationRepository;
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
