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
        private readonly CassandraContext _cassandraContext;
        public IMessageRepository MessageRepository { get; set; }
        public IConversationUserRepository ConversationUserRepository { get; set; }
        public IConversationRepository ConversationRepository { get; set; }
        public IMessageReactionUserRepository MessageReactionUserRepository { get; set; }

        public IPostRepository PostRepository { get; set; }
        public IFeedRepository FeedRepository { get; set; }
        public IUserRepository UserRepository { get; set; }      

        public IMessageAttachmentRepository MessageAttachmentRepository { get; set; }
        public INotificationRepository NotificationRepository { get; set; }
        public IPostReactionUserRepository PostReactionUserRepository { get; set; }
        public IFriendRequestRepository FriendRequestRepository { get; set; }
        public IUserRelationRepository UserRelationRepository { get; set; }
        public ICommentRepository CommentRepository { get; set; }
        public ICommentReactionUserRepository CommentReactionUserRepository { get; set; }
        public IGroupRepository GroupRepository { get; set; }
        public IGroupUserRepository GroupUserRepository { get; set; }


        public UnitOfWork(
            SocialNetworkDbContext context,
            CassandraContext cassandraContext,
            IMessageRepository messageRepository,
            IConversationUserRepository conversationUserRepository,
            IConversationRepository conversationRepository,
            IPostRepository postRepository,
            IFeedRepository feedRepository,
            IUserRepository userRepository,
            IMessageAttachmentRepository messageAttachmentRepository,
            IMessageReactionUserRepository messageReactionUserRepository,
            INotificationRepository notificationRepository,
            IPostReactionUserRepository postReactionUserRepository,
            IFriendRequestRepository friendRequestRepository,
            IUserRelationRepository userRelationRepository,
            ICommentRepository commentRepository,
            ICommentReactionUserRepository commentReactionUserRepository,
            IGroupRepository groupRepository,
            IGroupUserRepository groupUserRepository


        )
        {
            _context = context;
            _cassandraContext = cassandraContext;
            MessageRepository = messageRepository;
            ConversationUserRepository = conversationUserRepository;
            ConversationRepository = conversationRepository;
            MessageReactionUserRepository = messageReactionUserRepository;
            PostRepository = postRepository;
            FeedRepository = feedRepository;
            UserRepository = userRepository;
            MessageAttachmentRepository = messageAttachmentRepository;
            NotificationRepository = notificationRepository;
            PostReactionUserRepository = postReactionUserRepository;
            FriendRequestRepository = friendRequestRepository;
            UserRelationRepository = userRelationRepository;
            CommentRepository = commentRepository;
            CommentReactionUserRepository = commentReactionUserRepository;
            GroupRepository = groupRepository;
            GroupUserRepository = groupUserRepository;


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
