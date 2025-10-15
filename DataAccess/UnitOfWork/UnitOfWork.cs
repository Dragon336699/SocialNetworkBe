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
        public UnitOfWork(
            SocialNetworkDbContext context,
            IMessageRepository messageRepository,
            IConversationUserRepository conversationUserRepository
        )
        {
            _context = context;
            MessageRepository = messageRepository;
            ConversationUserRepository = conversationUserRepository;
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
