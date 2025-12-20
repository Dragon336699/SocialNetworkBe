using DataAccess.DbContext;
using Domain.Contracts.Responses.UserRelation;
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

        // Lấy danh sách người Follow mình (UserRelation: UserId (Họ) -> RelatedUserId (Mình))
        public async Task<(List<User> Users, int TotalCount)> GetFollowersAsync(Guid userId, int pageIndex, int pageSize)
        {
            var query = _context.UserRelation
                .Where(ur => ur.RelatedUserId == userId && ur.RelationType == UserRelationType.Following)
                .Include(ur => ur.User) // Include người thực hiện hành động follow
                .Select(ur => ur.User!); // Lấy ra User object

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }

        // Lấy danh sách mình đang Follow (UserRelation: UserId (Mình) -> RelatedUserId (Họ))
        public async Task<(List<User> Users, int TotalCount)> GetFollowingAsync(Guid userId, int pageIndex, int pageSize)
        {
            var query = _context.UserRelation
                .Where(ur => ur.UserId == userId && ur.RelationType == UserRelationType.Following)
                .Include(ur => ur.RelatedUser) // Include người được follow
                .Select(ur => ur.RelatedUser!);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }

        // Lấy danh sách bạn bè
        // Dựa vào logic cũ: Khi accept kết bạn, hệ thống tạo 2 bản ghi Friend 2 chiều.
        // Nên chỉ cần query UserId == Mình và Type == Friend là đủ.
        public async Task<(List<User> Users, int TotalCount)> GetFriendsAsync(Guid userId, int skip, int take)
        {
            var query = _context.UserRelation
                .Where(ur => (ur.UserId == userId) && ur.RelationType == UserRelationType.Friend)
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

        public List<MutualFriendIdsResponse> GetListIdsMutualFriends (Guid userId)
        {
            var myFriendIds = _context.UserRelation
                .Where(ur => ur.UserId == userId && ur.RelationType == UserRelationType.Friend)
                .Select(ur => ur.RelatedUserId)
                .ToList();

            var usersSentRequest = _context.FriendRequest
                .Where(fr => fr.SenderId == userId || fr.ReceiverId == userId)
                .Select(fr => fr.SenderId == userId ? fr.ReceiverId : fr.SenderId)
                .ToList();

            var suggestFriendIds = _context.UserRelation
                .Where(ur => myFriendIds.Contains(ur.UserId) && ur.RelationType == UserRelationType.Friend)
                .Where(ur => ur.RelatedUserId != userId)
                .Where(ur => !myFriendIds.Contains(ur.RelatedUserId))
                .Where(ur => !usersSentRequest.Contains(ur.RelatedUserId))
                .GroupBy(ur => ur.RelatedUserId)
                .Select(g => new MutualFriendIdsResponse
                {
                    SuggestedUserId = g.Key,
                    MutualFriendCount = g.Count()
                })
                .OrderByDescending(x => x.MutualFriendCount)
                .Take(10)
                .ToList();

            return suggestFriendIds;
        }
    }
}
