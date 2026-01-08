using DataAccess.DbContext;
using Domain.Interfaces.RepositoryInterfaces;

namespace DataAccess.Repositories.NoSQL
{
    public class InteractionRepository : IInteractionRepository
    {
        private readonly CassandraContext _context;
        public InteractionRepository(CassandraContext context)
        {
            _context = context;
        }

        public void IncreaseSearchCount (Guid userId, Guid targetUserId)
        {
            var queryCounter = "UPDATE user_interaction_counter SET search_count = search_count + 1 WHERE user_id = ? AND target_user_id = ?";
            var queryMeta = "UPDATE user_interaction_meta SET last_interaction = ? WHERE user_id = ? AND target_user_id = ?";
            var preparedCounter = _context.Session.Prepare(queryCounter);
            var preparedMeta = _context.Session.Prepare(queryMeta);
            var boundCounter = preparedCounter.Bind(userId, targetUserId);
            var boundMeta = preparedMeta.Bind(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), userId, targetUserId);
            _context.Session.Execute(boundCounter);
            _context.Session.Execute(boundMeta);
        }

        public void IncreaseViewCount(Guid userId, Guid targetUserId)
        {
            var queryCounter = "UPDATE user_interaction_counter SET view_count = view_count + 1 WHERE user_id = ? AND target_user_id = ?";
            var queryMeta = "UPDATE user_interaction_meta SET last_interaction = ? WHERE user_id = ? AND target_user_id = ?";
            var preparedCounter = _context.Session.Prepare(queryCounter);
            var preparedMeta = _context.Session.Prepare(queryMeta);
            var boundCounter = preparedCounter.Bind(userId, targetUserId);
            var boundMeta = preparedMeta.Bind(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), userId, targetUserId);
            _context.Session.Execute(boundCounter);
            _context.Session.Execute(boundMeta);
        }

        public void IncreaseLikeCount(Guid userId, Guid targetUserId)
        {
            var queryCounter = "UPDATE user_interaction_counter SET like_count = like_count + 1 WHERE user_id = ? AND target_user_id = ?";
            var queryMeta = "UPDATE user_interaction_meta SET last_interaction = ? WHERE user_id = ? AND target_user_id = ?";
            var preparedCounter = _context.Session.Prepare(queryCounter);
            var preparedMeta = _context.Session.Prepare(queryMeta);
            var boundCounter = preparedCounter.Bind(userId, targetUserId);
            var boundMeta = preparedMeta.Bind(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), userId, targetUserId);
            _context.Session.Execute(boundCounter);
            _context.Session.Execute(boundMeta);
        }

        public void InteractionPost(Guid userId, Guid postId, string action)
        {
            var userPostInteractionQuery = "INSERT INTO user_post_interaction (user_id, post_id, action, created_at) VALUES (?, ?, ?, ?)";
            var postUserInteractionQuery = "INSERT INTO post_user_interaction (post_id, user_id, action, created_at) VALUES (?, ?, ?, ?)";

            var preparedUserPost = _context.Session.Prepare(userPostInteractionQuery);
            var preparedPostUser = _context.Session.Prepare(postUserInteractionQuery);

            var boundUserPost = preparedUserPost.Bind(userId, postId, action, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var boundPostUser = preparedPostUser.Bind(postId, userId, action, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            _context.Session.Execute(boundUserPost);
            _context.Session.Execute(boundPostUser);
        }
    }
}
