using DataAccess.DbContext;
using Domain.Entities;
using Domain.Enum.User.Types;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class UserRelationRepository : GenericRepository<UserRelation>, IUserRelationRepository
    {
        public UserRelationRepository(SocialNetworkDbContext context) : base(context)
        {
        }

        public async Task<UserRelation?> GetRelationAsync(Guid userId, Guid relatedUserId, UserRelationType type)
        {
            return await _context.UserRelation
                .FirstOrDefaultAsync(ur => ur.UserId == userId &&
                                           ur.RelatedUserId == relatedUserId &&
                                           ur.RelationType == type);
        }

        public async Task<(List<User> Users, int TotalCount)> GetFollowersAsync(Guid userId, int skip, int take)
        {
            var query = _context.UserRelation
                .Where(ur => ur.RelatedUserId == userId && ur.RelationType == UserRelationType.Following)
                .Select(ur => ur.User!);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(List<User> Users, int TotalCount)> GetFollowingAsync(Guid userId, int skip, int take)
        {
            var query = _context.UserRelation
                .Where(ur => ur.UserId == userId && ur.RelationType == UserRelationType.Following)
                .Select(ur => ur.RelatedUser!);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return (items, totalCount);
        }

        // Lấy danh sách bạn bè
        // Dựa vào logic cũ: Khi accept kết bạn, hệ thống tạo 2 bản ghi Friend 2 chiều.
        // Nên chỉ cần query UserId == Mình và Type == Friend là đủ.
        public async Task<(List<User> Users, int TotalCount)> GetFriendsAsync(Guid userId, int skip, int take)
        {
            var query = _context.UserRelation
                .Where(ur => (ur.UserId == userId || ur.RelatedUserId == userId) && ur.RelationType == UserRelationType.Friend)
                .Include(ur => ur.User)
                .Include(ur => ur.RelatedUser)
                .AsSplitQuery()
                .Select(ur => ur.UserId == userId ? ur.RelatedUser : ur.User);

            var totalCount = await query.CountAsync();
            var items = await query.Skip(skip).Take(take).ToListAsync();
            return (items, totalCount);
        }

        public async Task<List<User>> GetFullFriends(Guid userId)
        {
            return await _context.UserRelation
                .Where(ur =>
                    (ur.UserId == userId || ur.RelatedUserId == userId) &&
                    ur.RelationType == UserRelationType.Friend)
                .Select(ur => ur.UserId == userId ? ur.RelatedUser : ur.User)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<UserRelation?> GetExistingRelationAsync(Guid userId, Guid relatedUserId)
        {
            return await _context.UserRelation
                .FirstOrDefaultAsync(x => x.UserId == userId && x.RelatedUserId == relatedUserId);
        }

    }
}
