using Domain.Contracts.Responses.Common;
using Domain.Contracts.Responses.User;
using Domain.Contracts.Responses.UserRelation;
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
        private readonly IUserService _userService;
        private readonly HttpClient _client;

        public UserRelationService(IUnitOfWork unitOfWork, ILogger<UserRelationService> logger, IUserService userService, IHttpClientFactory factory)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
            _client = factory.CreateClient("SuggestFriend");
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

        public async Task<UnfriendUserEnum> UnfriendUserAsync(Guid currentUserId, Guid targetUserId)
        {
            try
            {
                var relation1 = await _unitOfWork.UserRelationRepository
                    .GetRelationAsync(currentUserId, targetUserId, UserRelationType.Friend);

                var relation2 = await _unitOfWork.UserRelationRepository
                    .GetRelationAsync(targetUserId, currentUserId, UserRelationType.Friend);

                if (relation1 == null && relation2 == null)
                {
                    return UnfriendUserEnum.NotFriends;
                }

                if (relation1 != null) _unitOfWork.UserRelationRepository.Remove(relation1);
                if (relation2 != null) _unitOfWork.UserRelationRepository.Remove(relation2);

                var requestDirection1 = await _unitOfWork.FriendRequestRepository.GetFriendRequestAsync(currentUserId, targetUserId);
                if (requestDirection1 != null)
                {
                    _unitOfWork.FriendRequestRepository.Remove(requestDirection1);
                }

                var requestDirection2 = await _unitOfWork.FriendRequestRepository.GetFriendRequestAsync(targetUserId, currentUserId);
                if (requestDirection2 != null)
                {
                    _unitOfWork.FriendRequestRepository.Remove(requestDirection2);
                }

                var result = await _unitOfWork.CompleteAsync();

                return result > 0 ? UnfriendUserEnum.Success : UnfriendUserEnum.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unfriending user {TargetId} by {CurrentId}", targetUserId, currentUserId);
                return UnfriendUserEnum.Failed;
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

        public async Task<List<UserDto>> GetFollowersAsync(Guid userId, int skip, int take)
        {
            var (users, _) = await _unitOfWork.UserRelationRepository.GetFollowersAsync(userId, skip, take);
            return MapToUserDtos(users);
        }

        public async Task<List<UserDto>> GetFollowingAsync(Guid userId, int skip, int take)
        {
            var (users, _) = await _unitOfWork.UserRelationRepository.GetFollowingAsync(userId, skip, take);
            return MapToUserDtos(users);
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

        public async Task<List<MutualFriendReponse>> GetMutualFriends(Guid userId)
        {
            try
            {
                List<MutualFriendIdsResponse> mutualFriendIds = _unitOfWork.UserRelationRepository.GetListIdsMutualFriends(userId);
                List<MutualFriendReponse> mutualFriendList = new List<MutualFriendReponse>();
                SuggestUserDataDto? suggestUserData = await _client.GetFromJsonAsync<SuggestUserDataDto>($"/friend/recommend?user_id={userId}");
                if ( suggestUserData != null )
                {
                    foreach (var userRes in suggestUserData.recommendations)
                    {
                        UserDto? user = await _userService.GetUserInfoByUserId(userRes.target_user_id.ToString());
                        MutualFriendReponse mutualFriend = new MutualFriendReponse { MutualFriendCount = 0, User = user };
                        mutualFriendList.Add(mutualFriend);
                    }
                }
                
                foreach (var mutualFriendId in mutualFriendIds)
                {
                    UserDto? user = await _userService.GetUserInfoByUserId(mutualFriendId.SuggestedUserId.ToString());
                    MutualFriendReponse mutualFriend = new MutualFriendReponse { MutualFriendCount = mutualFriendId.MutualFriendCount, User = user };
                    mutualFriendList.Add(mutualFriend);
                }
                return mutualFriendList;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting mutual friends");
                throw;
            }
        }

        public async Task<BlockUserEnum> BlockUserAsync(Guid currentUserId, Guid targetUserId)
        {
            try
            {
                if (currentUserId == targetUserId) return BlockUserEnum.CannotBlockSelf;

                var targetUser = await _unitOfWork.UserRepository.GetByIdAsync(targetUserId);
                if (targetUser == null) return BlockUserEnum.TargetUserNotFound;
             
                var existingBlock = await _unitOfWork.UserRelationRepository
                    .GetRelationAsync(currentUserId, targetUserId, UserRelationType.Blocked);

                if (existingBlock != null) return BlockUserEnum.AlreadyBlocked;
               
                var friendRelation1 = await _unitOfWork.UserRelationRepository
                    .GetRelationAsync(currentUserId, targetUserId, UserRelationType.Friend);
                var friendRelation2 = await _unitOfWork.UserRelationRepository
                    .GetRelationAsync(targetUserId, currentUserId, UserRelationType.Friend);

                if (friendRelation1 != null) _unitOfWork.UserRelationRepository.Remove(friendRelation1);
                if (friendRelation2 != null) _unitOfWork.UserRelationRepository.Remove(friendRelation2);
            
                var followRelation1 = await _unitOfWork.UserRelationRepository
                    .GetRelationAsync(currentUserId, targetUserId, UserRelationType.Following);
                var followRelation2 = await _unitOfWork.UserRelationRepository
                    .GetRelationAsync(targetUserId, currentUserId, UserRelationType.Following);

                if (followRelation1 != null) _unitOfWork.UserRelationRepository.Remove(followRelation1);
                if (followRelation2 != null) _unitOfWork.UserRelationRepository.Remove(followRelation2);
              
                var friendRequest1 = await _unitOfWork.FriendRequestRepository
                    .GetFriendRequestAsync(currentUserId, targetUserId);
                var friendRequest2 = await _unitOfWork.FriendRequestRepository
                    .GetFriendRequestAsync(targetUserId, currentUserId);

                if (friendRequest1 != null) _unitOfWork.FriendRequestRepository.Remove(friendRequest1);
                if (friendRequest2 != null) _unitOfWork.FriendRequestRepository.Remove(friendRequest2);
               
                var blockRelation = new UserRelation
                {
                    UserId = currentUserId,
                    RelatedUserId = targetUserId,
                    RelationType = UserRelationType.Blocked,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _unitOfWork.UserRelationRepository.Add(blockRelation);
                return await _unitOfWork.CompleteAsync() > 0 ? BlockUserEnum.Success : BlockUserEnum.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking user {TargetId} by {CurrentId}", targetUserId, currentUserId);
                return BlockUserEnum.Failed;
            }
        }

        public async Task<UnblockUserEnum> UnblockUserAsync(Guid currentUserId, Guid targetUserId)
        {
            try
            {
                var targetUser = await _unitOfWork.UserRepository.GetByIdAsync(targetUserId);
                if (targetUser == null) return UnblockUserEnum.TargetUserNotFound;

                var blockRelation = await _unitOfWork.UserRelationRepository
                    .GetRelationAsync(currentUserId, targetUserId, UserRelationType.Blocked);

                if (blockRelation == null) return UnblockUserEnum.NotBlocked;

                _unitOfWork.UserRelationRepository.Remove(blockRelation);
                return await _unitOfWork.CompleteAsync() > 0 ? UnblockUserEnum.Success : UnblockUserEnum.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unblocking user {TargetId} by {CurrentId}", targetUserId, currentUserId);
                return UnblockUserEnum.Failed;
            }
        }

        public async Task<List<UserDto>> GetBlockedUsersAsync(Guid userId, int skip, int take)
        {
            var (relations, _) = await _unitOfWork.UserRelationRepository.GetBlockedUsersAsync(userId, skip, take);
            var users = relations.Select(r => r.RelatedUser).Where(u => u != null).ToList();
            return MapToUserDtos(users!);
        }
    }
}
