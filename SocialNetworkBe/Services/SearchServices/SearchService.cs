using AutoMapper;
using Domain.Contracts.Requests.Search;
using Domain.Contracts.Responses.Group;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.Search;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;
using Domain.Enum.Search.Types;
using Domain.Enum.Post.Types;

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
                        result.Users = await SearchUsersAsync(keywordNormalized, request.Skip, request.Take);
                        result.TotalUsersCount = result.Users?.Count ?? 0;
                        break;

                    case SearchType.Groups:
                        result.Groups = await SearchGroupsAsync(keywordNormalized, request.Skip, request.Take);
                        result.TotalGroupsCount = result.Groups?.Count ?? 0;
                        break;

                    case SearchType.Posts:
                        result.Posts = await SearchPostsAsync(keywordNormalized, request.Skip, request.Take);
                        result.TotalPostsCount = result.Posts?.Count ?? 0;
                        break;

                    case SearchType.All:
                    default:
                        result.Users = await SearchUsersAsync(keywordNormalized, 0, 5);
                        result.Groups = await SearchGroupsAsync(keywordNormalized, 0, 5);
                        result.Posts = await SearchPostsAsync(keywordNormalized, 0, 5);
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

        private async Task<List<UserDto>?> SearchUsersAsync(string keywordNormalized, int skip, int take)
        {
            var users = await _unitOfWork.UserRepository.SearchUsers(keywordNormalized);
            if (users == null || !users.Any()) return null;

            var paginatedUsers = users.Skip(skip).Take(take).ToList();
            return _mapper.Map<List<UserDto>>(paginatedUsers);
        }

        private async Task<List<GroupDto>?> SearchGroupsAsync(string keywordNormalized, int skip, int take)
        {
            var groups = await _unitOfWork.GroupRepository.SearchGroups(keywordNormalized);
            if (groups == null || !groups.Any()) return null;

            var paginatedGroups = groups.Skip(skip).Take(take).ToList();
            return _mapper.Map<List<GroupDto>>(paginatedGroups);
        }

        private async Task<List<PostDto>?> SearchPostsAsync(string keywordNormalized, int skip, int take)
        {           
            var postsByContent = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                p => p.Content.ToLower().Contains(keywordNormalized) &&
                     p.PostPrivacy != PostPrivacy.PendingApproval &&
                     p.PostPrivacy != PostPrivacy.Private &&
                     (p.GroupId == null || p.Group.IsPublic == 1),
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
                var userIds = matchedUsers.Select(u => u.Id).ToList();               
                var postsByUsers = await _unitOfWork.PostRepository.FindAsyncWithIncludesAndReactionUsers(
                    p => userIds.Contains(p.UserId) && 
                         p.PostPrivacy == PostPrivacy.Public &&
                         (p.GroupId == null || p.Group.IsPublic == 1),
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
    }
}
