using Domain.Contracts.Responses.UserRelation;
using Domain.Entities;
using Domain.Enum.User.Types;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IUserRelationRepository : IGenericRepository<UserRelation>
    {
        Task<UserRelation?> GetRelationAsync(Guid userId, Guid relatedUserId, UserRelationType type);

        Task<(List<User> Users, int TotalCount)> GetFollowersAsync(Guid userId, int skip, int take);

        Task<(List<User> Users, int TotalCount)> GetFollowingAsync(Guid userId, int skip, int take);

        Task<(List<User> Users, int TotalCount)> GetFriendsAsync(Guid userId, int skip, int take, string? keySearch);
        Task<List<User>> GetFullFriends(Guid userId);
        Task<UserRelation?> GetExistingRelationAsync(Guid userId, Guid relatedUserId);
        List<MutualFriendIdsResponse> GetListIdsMutualFriends(Guid userId);
    }
}
