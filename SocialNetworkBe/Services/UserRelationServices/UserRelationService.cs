using Domain.Contracts.Responses.Common;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.User.Types;
using Domain.Enum.UserRelation.Funtions;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace SocialNetworkBe.Services.UserRelationServices
{
    public class UserRelationService : IUserRelationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserRelationService> _logger;

        public UserRelationService(IUnitOfWork unitOfWork, ILogger<UserRelationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<FollowUserEnum> FollowUserAsync(Guid currentUserId, Guid targetUserId)
        {
            try
            {
                if (currentUserId == targetUserId) return FollowUserEnum.CannotFollowSelf;

                var targetUser = await _unitOfWork.UserRepository.GetByIdAsync(targetUserId);
                if (targetUser == null) return FollowUserEnum.TargetUserNotFound;

                // Kiểm tra đã follow chưa
                var existingRelation = await _unitOfWork.UserRelationRepository
                    .GetRelationAsync(currentUserId, targetUserId, UserRelationType.Following);

                if (existingRelation != null) return FollowUserEnum.AlreadyFollowing;

                // Tạo quan hệ mới
                var relation = new UserRelation
                {
                    UserId = currentUserId,
                    RelatedUserId = targetUserId,
                    RelationType = UserRelationType.Following,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _unitOfWork.UserRelationRepository.Add(relation);
                return await _unitOfWork.CompleteAsync() > 0 ? FollowUserEnum.Success : FollowUserEnum.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error following user");
                return FollowUserEnum.Failed;
            }
        }

        public async Task<UnfollowUserEnum> UnfollowUserAsync(Guid currentUserId, Guid targetUserId)
        {
            try
            {
                var relation = await _unitOfWork.UserRelationRepository
                    .GetRelationAsync(currentUserId, targetUserId, UserRelationType.Following);

                if (relation == null) return UnfollowUserEnum.NotFollowing;

                _unitOfWork.UserRelationRepository.Remove(relation);
                return await _unitOfWork.CompleteAsync() > 0 ? UnfollowUserEnum.Success : UnfollowUserEnum.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unfollowing user");
                return UnfollowUserEnum.Failed;
            }
        }

        private List<UserDto> MapToUserDtos(List<User> users)
        {
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName ?? "",
                Status = u.Status.ToString(),
                FirstName = u.FirstName,
                LastName = u.LastName,
                AvatarUrl = u.AvatarUrl
            }).ToList();
        }

        public async Task<PagedResponse<UserDto>> GetFollowersAsync(Guid userId, int pageIndex, int pageSize)
        {
            var (users, totalCount) = await _unitOfWork.UserRelationRepository.GetFollowersAsync(userId, pageIndex, pageSize);
            return new PagedResponse<UserDto>(MapToUserDtos(users), pageIndex, pageSize, totalCount);
        }

        public async Task<PagedResponse<UserDto>> GetFollowingAsync(Guid userId, int pageIndex, int pageSize)
        {
            var (users, totalCount) = await _unitOfWork.UserRelationRepository.GetFollowingAsync(userId, pageIndex, pageSize);
            return new PagedResponse<UserDto>(MapToUserDtos(users), pageIndex, pageSize, totalCount);
        }

        public async Task<List<UserDto>> GetFriendsAsync(Guid userId, int skip, int take)
        {
            var (users, totalCount) = await _unitOfWork.UserRelationRepository.GetFriendsAsync(userId, skip, take);
            var userDtos = MapToUserDtos(users);
            return userDtos;
        }

        public async Task<List<UserDto>> GetFullFriends(Guid userId)
        {
            var users = await _unitOfWork.UserRelationRepository.GetFullFriends(userId);
            var userDtos = MapToUserDtos(users);
            return userDtos;
        }
    }
}
