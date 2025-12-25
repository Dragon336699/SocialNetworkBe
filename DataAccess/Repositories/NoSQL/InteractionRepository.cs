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
    }
}
