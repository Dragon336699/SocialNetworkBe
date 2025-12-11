using DataAccess.DbContext;
using Domain.Contracts.Responses.Post;
using Domain.Contracts.Responses.Post.UserFeed;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Post.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace SocialNetworkBe.Services.FeedServices
{
    public class FeedService : IFeedService
    {
        private readonly IUnitOfWork _unitOfWokrk;
        private readonly IPostService _postService;
        private readonly IUserRelationService _userRelationService;
        private readonly ILogger<FeedService> _logger;
        public FeedService(IUnitOfWork unitOfWork, IPostService postService, IUserRelationService userRelationService, ILogger<FeedService> logger)
        {
            _unitOfWokrk = unitOfWork;
            _logger = logger;
            _postService = postService;
            _userRelationService = userRelationService;
        }

        public async Task FeedForPost(Guid postId, Guid authorId)
        {
            try
            {
                List<UserDto> userDtos = await _userRelationService.GetFullFriends(authorId);
                List<Guid> friendIds = new List<Guid>();
                friendIds.AddRange(userDtos.Select(x => x.Id));
                friendIds.Add(authorId);
                _unitOfWokrk.FeedRepository.FeedForPost(postId, friendIds, authorId);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error while feed for post {PostId}", postId);
                throw;
            }
        }

        public async Task<(GetAllPostsEnum, List<PostDto>)> GetFeedsForUser(Guid userId)
        {
            try
            {
                List<UserFeedResponse> userFeeds = await _unitOfWokrk.FeedRepository.GetFeedsForUser(userId);
                List<PostDto> posts = new List<PostDto>();
                foreach (var feed in userFeeds)
                {
                    var (result, post) = await _postService.GetPostByIdAsync(feed.PostId, feed.UserId);
                    if (post != null) posts.Add(post);
                }
                return (GetAllPostsEnum.Success, posts);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting for user {UserId}", userId);
                throw;
            }
        }
    }
}
