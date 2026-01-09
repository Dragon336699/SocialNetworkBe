using AutoMapper;
using Domain.Contracts.Requests.Search;
using Domain.Contracts.Requests.UserRelation;
using Domain.Contracts.Responses.Group;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.Search;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Group.Types;
using Domain.Enum.Post.Types;
using Domain.Enum.Search.Types;
using Domain.Enum.User.Types;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace SocialNetworkBe.Services.SearchServices
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SearchService> _logger;

        public SearchService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SearchService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SearchResultDto?> SearchAsync(SearchRequest request, Guid userId, bool saveHistory = false)
        {
            try
            {
                string keywordNormalized = request.Keyword.Trim().ToLower();
                var result = new SearchResultDto();

                switch (request.Type)
                {
                    case SearchType.Users:
                        result.Users = await SearchUsersAsync(keywordNormalized, userId, request.Skip, request.Take);
                        result.TotalUsersCount = result.Users?.Count ?? 0;
                        break;

                    case SearchType.Groups:
                        result.Groups = await SearchGroupsAsync(keywordNormalized, userId, request.Skip, request.Take);
                        result.TotalGroupsCount = result.Groups?.Count ?? 0;
                        break;

                    case SearchType.Posts:
                        result.Posts = await SearchPostsAsync(keywordNormalized, userId, request.Skip, request.Take);
                        result.TotalPostsCount = result.Posts?.Count ?? 0;
                        break;

                    case SearchType.All:
                    default:
                        result.Users = await SearchUsersAsync(keywordNormalized, userId, 0, 5);
                        result.Groups = await SearchGroupsAsync(keywordNormalized, userId, 0, 5);
                        result.Posts = await SearchPostsAsync(keywordNormalized, userId, 0, 5);
                        result.TotalUsersCount = result.Users?.Count ?? 0;
                        result.TotalGroupsCount = result.Groups?.Count ?? 0;
                        result.TotalPostsCount = result.Posts?.Count ?? 0;
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while searching with keyword: {Keyword}", request.Keyword);
                return null;
            }
        }

        public async Task<bool> SaveSearchHistoryAsync(Guid userId, SaveSearchHistoryRequest request)
        {
            try
            {
                var contentTrimmed = request.Content.Trim();
               
                var existingHistory = await _unitOfWork.SearchingHistoryRepository
                    .FindFirstAsync(sh => sh.UserId == userId && sh.Content == contentTrimmed);

                if (existingHistory != null)
                {
                    _unitOfWork.SearchingHistoryRepository.Remove(existingHistory);
                    await _unitOfWork.CompleteAsync();
                }

                var searchHistory = new SearchingHistory
                {
                    Id = Guid.NewGuid(),
                    Content = contentTrimmed,
                    ImageUrl = request.ImageUrl,
                    NavigateUrl = request.NavigateUrl,
                    UserId = userId
                };

                _unitOfWork.SearchingHistoryRepository.Add(searchHistory);
                var result = await _unitOfWork.CompleteAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving search history for user {UserId}", userId);
                return false;
            }
        }

        public async Task<List<SearchHistoryDto>?> GetRecentSearchesAsync(Guid userId, int take = 10)
        {
            try
            {
                var histories = await _unitOfWork.SearchingHistoryRepository.GetRecentSearchesByUserAsync(userId, take);
                if (histories == null || !histories.Any()) return null;

                var historyDtos = histories.Select(h => new SearchHistoryDto
                {
                    Id = h.Id,
                    Content = h.Content,
                    ImageUrl = h.ImageUrl,
                    NavigateUrl = h.NavigateUrl
                }).ToList();

                return historyDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent searches for user {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> DeleteSearchHistoryAsync(Guid userId, Guid historyId)
        {
            try
            {
                var history = await _unitOfWork.SearchingHistoryRepository
                    .FindFirstAsync(h => h.Id == historyId && h.UserId == userId);

                if (history == null) return false;

                _unitOfWork.SearchingHistoryRepository.Remove(history);
                var result = await _unitOfWork.CompleteAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting search history {HistoryId}", historyId);
                return false;
            }
        }

        public async Task<bool> ClearAllSearchHistoryAsync(Guid userId)
        {
            try
            {
                var histories = await _unitOfWork.SearchingHistoryRepository
                    .FindAsync(h => h.UserId == userId);

                if (histories == null || !histories.Any()) return true;

                _unitOfWork.SearchingHistoryRepository.RemoveRange(histories);
                var result = await _unitOfWork.CompleteAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing search history for user {UserId}", userId);
                return false;
            }
        }

        private async Task<List<UserDto>?> SearchUsersAsync(string keywordNormalized, Guid currentUserId, int skip, int take)
        {
            var users = await _unitOfWork.UserRepository.SearchUsers(keywordNormalized);
            if (users == null || !users.Any()) return null;

            var blockedUserIds = await GetBlockedUserIdsAsync(currentUserId);
       
            var filteredUsers = users.Where(u => !blockedUserIds.Contains(u.Id)).ToList();
            if (!filteredUsers.Any()) return null;

            var paginatedUsers = filteredUsers.Skip(skip).Take(take).ToList();
            return _mapper.Map<List<UserDto>>(paginatedUsers);
        }

        private async Task<List<GroupDto>?> SearchGroupsAsync(string keywordNormalized, Guid userId, int skip, int take)
        {
            var groups = await _unitOfWork.GroupRepository.SearchGroups(keywordNormalized);
            if (groups == null || !groups.Any()) return null;

            var filteredGroups = groups.Where(g =>
                !g.GroupUsers.Any(gu => gu.UserId == userId && gu.RoleName == GroupRole.Banned)
            ).ToList();

            if (!filteredGroups.Any()) return null;

            var paginatedGroups = filteredGroups.Skip(skip).Take(take).ToList();
            return _mapper.Map<List<GroupDto>>(paginatedGroups);
        }

        private async Task<List<PostDto>?> SearchPostsAsync(string keywordNormalized, Guid userId, int skip, int take)
        {
            var bannedGroupIds = await GetBannedGroupIdsAsync(userId);
            var blockedUserIds = await GetBlockedUserIdsAsync(userId);

            var postsByContent = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                p => p.Content.ToLower().Contains(keywordNormalized) &&
                     p.PostPrivacy != PostPrivacy.PendingApproval &&
                     p.PostPrivacy != PostPrivacy.Private &&
                     (p.GroupId == null || p.Group.IsPublic == 1) &&
                     (!p.GroupId.HasValue || !bannedGroupIds.Contains(p.GroupId.Value)) &&
                     !blockedUserIds.Contains(p.UserId),
                p => p.User,
                p => p.PostImages,
                p => p.Group
            );

            if (postsByContent != null && postsByContent.Any())
            {
                var paginatedPosts = postsByContent
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
                return _mapper.Map<List<PostDto>>(paginatedPosts);
            }

            var matchedUsers = await _unitOfWork.UserRepository.SearchUsers(keywordNormalized);
            if (matchedUsers != null && matchedUsers.Any())
            {
                var userIds = matchedUsers
                    .Where(u => !blockedUserIds.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToList();
                var postsByUsers = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                    p => userIds.Contains(p.UserId) && 
                         p.PostPrivacy == PostPrivacy.Public &&
                         (p.GroupId == null || p.Group.IsPublic == 1) &&
                         (!p.GroupId.HasValue || !bannedGroupIds.Contains(p.GroupId.Value)),
                    p => p.User,
                    p => p.PostImages,
                    p => p.Group
                );

                if (postsByUsers != null && postsByUsers.Any())
                {
                    var paginatedPosts = postsByUsers
                        .OrderByDescending(p => p.CreatedAt)
                        .Skip(skip)
                        .Take(take)
                        .ToList();
                    return _mapper.Map<List<PostDto>>(paginatedPosts);
                }
            }
            return null;
        }

        private async Task<List<Guid>> GetBannedGroupIdsAsync(Guid userId)
        {
            try
            {
                var bannedGroupUsers = await _unitOfWork.GroupUserRepository.FindAsync(
                    gu => gu.UserId == userId && gu.RoleName == GroupRole.Banned
                );

                if (bannedGroupUsers == null || !bannedGroupUsers.Any())
                {
                    return new List<Guid>();
                }

                return bannedGroupUsers.Select(gu => gu.GroupId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting banned groups for user {UserId}", userId);
                return new List<Guid>();
            }
        }

        private async Task<List<Guid>> GetBlockedUserIdsAsync(Guid userId)
        {
            try
            {
                var blockedByMe = await _unitOfWork.UserRelationRepository.FindAsync(
                    ur => ur.UserId == userId && ur.RelationType == UserRelationType.Blocked
                );
            
                var blockedMe = await _unitOfWork.UserRelationRepository.FindAsync(
                    ur => ur.RelatedUserId == userId && ur.RelationType == UserRelationType.Blocked
                );

                var blockedIds = new List<Guid>();

                if (blockedByMe != null && blockedByMe.Any())
                {
                    blockedIds.AddRange(blockedByMe.Select(ur => ur.RelatedUserId));
                }

                if (blockedMe != null && blockedMe.Any())
                {
                    blockedIds.AddRange(blockedMe.Select(ur => ur.UserId));
                }

                return blockedIds.Distinct().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blocked users for user {UserId}", userId);
                return new List<Guid>();
            }
        }
    }
}
