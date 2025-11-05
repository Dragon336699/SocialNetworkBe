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
        public IMessageReactionUserRepository MessageReactionUserRepository { get; set; }

        public IPostRepository PostRepository { get; set; }
        public IUserRepository UserRepository { get; set; }      

        public IMessageAttachmentRepository MessageAttachmentRepository { get; set; }
        public IPostReactionUserRepository PostReactionUserRepository { get; set; }

        public UnitOfWork(
            SocialNetworkDbContext context,
            IMessageRepository messageRepository,
            IConversationUserRepository conversationUserRepository,
            IConversationRepository conversationRepository,
            IPostRepository postRepository,           
            IUserRepository userRepository,
            IMessageAttachmentRepository messageAttachmentRepository,
            IMessageReactionUserRepository messageReactionUserRepository,
            IPostReactionUserRepository postReactionUserRepository

        )
        {
            _context = context;
            MessageRepository = messageRepository;
            ConversationUserRepository = conversationUserRepository;
            ConversationRepository = conversationRepository;
            MessageReactionUserRepository = messageReactionUserRepository;
            PostRepository = postRepository;           
            UserRepository = userRepository;
            MessageAttachmentRepository = messageAttachmentRepository;
            PostReactionUserRepository = postReactionUserRepository;

        }
        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
