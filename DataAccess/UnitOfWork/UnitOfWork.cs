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

        public IPostRepository PostRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IPostImageRepository PostImageRepository { get; set; }

        public IMessageAttachmentRepository MessageAttachmentRepository { get; set; }
        public UnitOfWork(
            SocialNetworkDbContext context,
            IMessageRepository messageRepository,
            IConversationUserRepository conversationUserRepository,
            IConversationRepository conversationRepository,
            IPostRepository postRepository,
            IPostImageRepository postImageRepository,
            IUserRepository userRepository,


            IMessageAttachmentRepository messageAttachmentRepository

        )
        {
            _context = context;
            MessageRepository = messageRepository;
            ConversationUserRepository = conversationUserRepository;
            ConversationRepository = conversationRepository;

            PostRepository = postRepository;
            PostImageRepository = postImageRepository;
            UserRepository = userRepository;

            MessageAttachmentRepository = messageAttachmentRepository;

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
