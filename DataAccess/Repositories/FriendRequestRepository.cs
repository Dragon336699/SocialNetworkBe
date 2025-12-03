using DataAccess.DbContext;
using Domain.Entities;
using Domain.Enum.FriendRequest.Functions;
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
    public class FriendRequestRepository : GenericRepository<FriendRequest>, IFriendRequestRepository
    {
        public FriendRequestRepository(SocialNetworkDbContext context) : base(context)
        {
        }

        public async Task<FriendRequest?> GetFriendRequestAsync(Guid senderId, Guid receiverId)
        {
            try
            {
                return await _context.FriendRequest
                    .Include(fr => fr.Sender)
                    .Include(fr => fr.Receiver)
                    .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AreFriendsAsync(Guid userId1, Guid userId2)
        {
            try
            {               
                var acceptedRequest = await _context.FriendRequest
                    .AnyAsync(fr =>
                        ((fr.SenderId == userId1 && fr.ReceiverId == userId2) ||
                         (fr.SenderId == userId2 && fr.ReceiverId == userId1)) &&
                        fr.FriendRequestStatus == FriendRequestStatus.Accepted.ToString());
              
                var userRelation = await _context.UserRelation
                    .AnyAsync(ur =>
                        ((ur.UserId == userId1 && ur.RelatedUserId == userId2) ||
                         (ur.UserId == userId2 && ur.RelatedUserId == userId1)) &&
                        ur.RelationType == UserRelationType.Friend);

                return acceptedRequest || userRelation;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<(List<FriendRequest> Items, int TotalCount)> GetSentFriendRequestsAsync(Guid senderId, int pageIndex, int pageSize)
        {
            try
            {
                var query = _context.FriendRequest
                    .Include(fr => fr.Receiver)
                    .Where(fr => fr.SenderId == senderId &&
                                 fr.FriendRequestStatus == FriendRequestStatus.Pending.ToString());

                var totalCount = await query.CountAsync();

                 query = query.OrderByDescending(fr => fr.CreatedAt);
                //query = query.OrderByDescending(fr => fr.Id);

                var items = await query
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (items, totalCount);
            }
            catch (Exception)
            {
                return (new List<FriendRequest>(), 0);
            }
        }

        public async Task<(List<FriendRequest> Items, int TotalCount)> GetReceivedFriendRequestsAsync(Guid receiverId, int pageIndex, int pageSize)
        {
            try
            {
                var query = _context.FriendRequest
                    .Include(fr => fr.Sender)
                    .Where(fr => fr.ReceiverId == receiverId &&
                                 fr.FriendRequestStatus == FriendRequestStatus.Pending.ToString());

                var totalCount = await query.CountAsync();
                 query = query.OrderByDescending(fr => fr.CreatedAt);

                var items = await query
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (items, totalCount);
            }
            catch (Exception)
            {
                return (new List<FriendRequest>(), 0);
            }
        }
    }
}
