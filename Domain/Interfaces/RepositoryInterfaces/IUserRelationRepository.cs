using Domain.Entities;
using Domain.Enum.User.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IUserRelationRepository : IGenericRepository<UserRelation>
    {
        Task<UserRelation?> GetRelationAsync(Guid userId, Guid relatedUserId, UserRelationType type);

        Task<(List<User> Users, int TotalCount)> GetFollowersAsync(Guid userId, int skip, int take);

        Task<(List<User> Users, int TotalCount)> GetFollowingAsync(Guid userId, int skip, int take);

        Task<(List<User> Users, int TotalCount)> GetFriendsAsync(Guid userId, int skip, int take);
        Task<List<User>> GetFullFriends(Guid userId);
        Task<UserRelation?> GetExistingRelationAsync(Guid userId, Guid relatedUserId);
    }
}
