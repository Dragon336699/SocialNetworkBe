using DataAccess.DbContext;
using Domain.Entities;
using Domain.Enum.User.Types;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<(List<User> Users, int TotalCount)> GetFriendsAsync(Guid userId, int pageIndex, int pageSize)
        {
            var query = _context.UserRelation
                .Where(ur => ur.UserId == userId && ur.RelationType == UserRelationType.Friend)
                .Include(ur => ur.RelatedUser)
                .Select(ur => ur.RelatedUser!);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }
    }
}
