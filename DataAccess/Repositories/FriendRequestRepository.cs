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
        public async Task<bool> DeleteFriendRequestAsync(FriendRequest request)
        {
            _context.FriendRequest.Remove(request);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<FriendRequest>> GetSentFriendRequestsAsync(Guid senderId)
        {
            return await _context.FriendRequest
                .Include(fr => fr.Receiver)
                .Where(fr => fr.SenderId == senderId && fr.FriendRequestStatus == FriendRequestStatus.Pending.ToString())
                .ToListAsync();
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
    }
}
