using Azure.Core;
using Domain.Contracts.Requests.Post;
using Domain.Contracts.Responses.Feed;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Entities.NoSQL;
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
        private readonly IInteractionService _interactionService;
        public FeedService(IUnitOfWork unitOfWork, IPostService postService, IUserRelationService userRelationService, ILogger<FeedService> logger, IInteractionService interactionService)
        {
            _unitOfWokrk = unitOfWork;
            _logger = logger;
            _postService = postService;
            _userRelationService = userRelationService;
            _interactionService = interactionService;
        }

        public async Task FeedForPost(Guid postId, Guid authorId)
        {
            try
            {
                List<UserDto> userDtos = await _userRelationService.GetFullFriends(authorId);
                List<Guid> friendIds = new List<Guid>();
                friendIds.AddRange(userDtos.Select(x => x.Id));
                friendIds.Add(authorId);
                _unitOfWokrk.FeedRepository.FeedForPost(postId, friendIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while feed for post {PostId}", postId);
                throw;
            }
        }

        public async Task<(GetAllPostsEnum, List<FeedDto>)> GetFeedsForUser(Guid userId)
        {
            try
            {
                List<UserFeedUnseen> userFeeds = await _unitOfWokrk.FeedRepository.GetFeedsForUser(userId);
                List<FeedDto> feeds = new List<FeedDto>();
                foreach (var feed in userFeeds)
                {
                    var (result, post) = await _postService.GetPostByIdAsync(feed.PostId, feed.UserId);

                    if (post != null)
                    {
                        FeedDto feedTemp = new FeedDto
                        {
                            CreatedAt = feed.CreatedAt,
                            Post = post
                        };

                        feeds.Add(feedTemp);
                    };
                }
                return (GetAllPostsEnum.Success, feeds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting for user {UserId}", userId);
                throw;
            }
        }

        public void SeenFeed(List<SeenFeedRequest> request, Guid userId)
        {
            try
            {
                foreach (var post in request)
                {
                    _interactionService.InteractionPost(userId, post.PostId, "view");
                }

                _unitOfWokrk.FeedRepository.SeenFeed(request, userId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
