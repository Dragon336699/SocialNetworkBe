using Domain.Contracts.Requests.Post;
using Domain.Contracts.Responses.Feed;
using Domain.Contracts.Responses.Post;
using Domain.Entities;
using Domain.Enum.Post.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/post")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IFeedService _feedService;

        public PostController(IPostService postService, IFeedService feedService)
        {
            _postService = postService;
            _feedService = feedService;

        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request) 
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));              
                var (status, postDto) = await _postService.CreatePostAsync(request, userId);

                return status switch
                {
                    CreatePostEnum.UserNotFound => BadRequest(new CreatePostResponse { Message = status.GetMessage() }),
                    CreatePostEnum.InvalidContent => BadRequest(new CreatePostResponse { Message = status.GetMessage() }),
                    CreatePostEnum.InvalidImageFormat => BadRequest(new CreatePostResponse { Message = status.GetMessage() }),
                    CreatePostEnum.FileTooLarge => BadRequest(new CreatePostResponse { Message = status.GetMessage() }),
                    CreatePostEnum.ImageUploadFailed => BadRequest(new CreatePostResponse { Message = status.GetMessage() }),
                    CreatePostEnum.CreatePostSuccess => Ok(new CreatePostResponse
                    {
                        Message = status.GetMessage(),
                        data = postDto
                    }),
                    _ => StatusCode(500, new CreatePostResponse { Message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("getNewsFeed")]
        public async Task<IActionResult> GetPostsForNewFeed([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return BadRequest(new { message = "User not found" });

                var (status, feeds) = await _feedService.GetFeedsForUser(Guid.Parse(userId));   

                return status switch
                {
                    GetAllPostsEnum.Success => Ok(new { Message = status.GetMessage(), Posts = feeds, TotalCount = feeds?.Count ?? 0 }),
                    GetAllPostsEnum.NoPostsFound => Ok(new { Message = status.GetMessage(), Posts = new List<FeedDto>(), TotalCount = 0 }),
                    GetAllPostsEnum.Failed => BadRequest(new GetAllPostsResponse { Message = status.GetMessage(), Posts = null, TotalCount = 0 }),
                    _ => StatusCode(500, new GetAllPostsResponse { Message = "Unknown error occurred", Posts = null, TotalCount = 0 })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetAllPostsResponse
                {
                    Message = ex.Message,
                    Posts = null,
                    TotalCount = 0
                });
            }
        }

        [Authorize]
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPostById(Guid postId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, postDto) = await _postService.GetPostByIdAsync(postId, userId);

                return status switch
                {
                    GetPostByIdEnum.Success => Ok(new GetPostByIdResponse
                    {
                        Message = status.GetMessage(),
                        Post = postDto
                    }),
                    GetPostByIdEnum.PostNotFound => NotFound( new GetPostByIdResponse{ Message = status.GetMessage() } ),              
                    GetPostByIdEnum.Failed => StatusCode( 500, new GetPostByIdResponse{ Message = status.GetMessage() } ),
                    _ => StatusCode(500, new GetPostByIdResponse{ Message = "Unknown error occurred"} )
                };
            }
            catch (Exception ex)
            {
                return StatusCode( 500, new GetPostByIdResponse{ Message = ex.Message } );
            }
        }

        [Authorize]
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost([FromForm] UpdatePostRequest request, Guid postId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, postDto) = await _postService.UpdatePostAsync(postId, request, userId);

                return status switch
                {
                    UpdatePostEnum.PostNotFound => NotFound(new UpdatePostResponse { Message = status.GetMessage() }),
                    UpdatePostEnum.InvalidContent => BadRequest(new UpdatePostResponse { Message = status.GetMessage() }),
                    UpdatePostEnum.InvalidImageFormat => BadRequest(new UpdatePostResponse { Message = status.GetMessage() }),
                    UpdatePostEnum.FileTooLarge => BadRequest(new UpdatePostResponse { Message = status.GetMessage() }),
                    UpdatePostEnum.ImageUploadFailed => BadRequest(new UpdatePostResponse { Message = status.GetMessage() }),
                    UpdatePostEnum.UpdatePostSuccess => Ok(new UpdatePostResponse
                    {
                        Message = status.GetMessage(),
                        Post = postDto
                    }),
                    UpdatePostEnum.UpdatePostFailed => StatusCode(500, new UpdatePostResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new UpdatePostResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UpdatePostResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _postService.DeletePostAsync(postId, userId);

                return status switch
                {
                    DeletePostEnum.PostNotFound => NotFound(new DeletePostResponse { Message = status.GetMessage() }),
                    DeletePostEnum.DeletePostSuccess => Ok(new DeletePostResponse { Message = status.GetMessage() }),
                    DeletePostEnum.DeletePostFailed => StatusCode(500, new DeletePostResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new DeletePostResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DeletePostResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("reaction")]
        public async Task<IActionResult> AddUpdateDeleteReactionPost([FromBody] ReactionPostRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var updatedPost = await _postService.AddUpdateDeleteReactionPost(request, userId);

                if (updatedPost == null)
                    return BadRequest(new ReactionPostResponse { Message = "Reaction failed" });

                return Ok(new { Message = "Reaction successfully", data = updatedPost });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ReactionPostResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPostsByUserId(Guid userId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, posts) = await _postService.GetPostsByUserIdAsync(userId, currentUserId, skip, take);

                return status switch
                {
                    GetPostsByUserEnum.Success => Ok(new GetAllPostsResponse { Message = status.GetMessage(), Posts = posts, TotalCount = posts?.Count ?? 0 }),
                    GetPostsByUserEnum.NoPostsFound => Ok(new GetAllPostsResponse { Message = status.GetMessage(), Posts = new List<PostDto>(), TotalCount = 0 }),
                    GetPostsByUserEnum.Failed => BadRequest(new GetAllPostsResponse { Message = status.GetMessage(), Posts = null, TotalCount = 0 }),
                    _ => StatusCode(500, new GetAllPostsResponse { Message = "Unknown error occurred.", Posts = null, TotalCount = 0 })

                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetAllPostsResponse
                {
                    Message = ex.Message,
                    Posts = null,
                    TotalCount = 0
                });
            }
        }

        [Authorize]
        [HttpPost("seen")]
        public IActionResult SeenPost([FromBody] List<SeenFeedRequest> request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();
                _feedService.SeenFeed(request, Guid.Parse(userId));

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message
                });
            }
        }
       
        [Authorize]
        [HttpGet("group/{groupId}/pending")]
        public async Task<IActionResult> GetPendingPosts(Guid groupId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, posts) = await _postService.GetPendingPostsAsync(groupId, currentUserId, skip, take);

                return status switch
                {
                    GetPendingPostsEnum.Success => Ok(new GetPendingPostsResponse { Message = status.GetMessage(), Posts = posts, TotalCount = posts?.Count ?? 0 }),
                    GetPendingPostsEnum.GroupNotFound => NotFound(new GetPendingPostsResponse { Message = status.GetMessage(), Posts = null, TotalCount = 0 }),
                    GetPendingPostsEnum.Unauthorized => StatusCode(403, new GetPendingPostsResponse { Message = status.GetMessage(), Posts = null, TotalCount = 0 }),
                    GetPendingPostsEnum.NoPendingPosts => Ok(new GetPendingPostsResponse { Message = status.GetMessage(), Posts = new List<PostDto>(), TotalCount = 0 }),
                    GetPendingPostsEnum.Failed => StatusCode(500, new GetPendingPostsResponse { Message = status.GetMessage(), Posts = null, TotalCount = 0 }),
                    _ => StatusCode(500, new GetPendingPostsResponse { Message = "Unknown error occurred", Posts = null, TotalCount = 0 })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetPendingPostsResponse
                {
                    Message = ex.Message,
                    Posts = null,
                    TotalCount = 0
                });
            }
        }

        [Authorize]
        [HttpPost("{postId}/approve")]
        public async Task<IActionResult> ApprovePost(Guid postId)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, postDto) = await _postService.ApprovePostAsync(postId, currentUserId);

                return status switch
                {
                    ApprovePostEnum.Success => Ok(new ApprovePostResponse { Message = status.GetMessage(), Post = postDto }),
                    ApprovePostEnum.PostNotFound => NotFound(new ApprovePostResponse { Message = status.GetMessage() }),
                    ApprovePostEnum.PostNotPending => BadRequest(new ApprovePostResponse { Message = status.GetMessage() }),
                    ApprovePostEnum.GroupNotFound => NotFound(new ApprovePostResponse { Message = status.GetMessage() }),
                    ApprovePostEnum.Unauthorized => StatusCode(403, new ApprovePostResponse { Message = status.GetMessage() }),
                    ApprovePostEnum.Failed => StatusCode(500, new ApprovePostResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new ApprovePostResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApprovePostResponse { Message = ex.Message });
            }
        }
       
        [Authorize]
        [HttpPost("{postId}/reject")]
        public async Task<IActionResult> RejectPost(Guid postId)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _postService.RejectPostAsync(postId, currentUserId);

                return status switch
                {
                    RejectPostEnum.Success => Ok(new RejectPostResponse { Message = status.GetMessage() }),
                    RejectPostEnum.PostNotFound => NotFound(new RejectPostResponse { Message = status.GetMessage() }),
                    RejectPostEnum.PostNotPending => BadRequest(new RejectPostResponse { Message = status.GetMessage() }),
                    RejectPostEnum.GroupNotFound => NotFound(new RejectPostResponse { Message = status.GetMessage() }),
                    RejectPostEnum.Unauthorized => StatusCode(403, new RejectPostResponse { Message = status.GetMessage() }),
                    RejectPostEnum.Failed => StatusCode(500, new RejectPostResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new RejectPostResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RejectPostResponse { Message = ex.Message });
            }
        }
      
        [Authorize]
        [HttpGet("my-pending")]
        public async Task<IActionResult> GetMyPendingPosts([FromQuery] Guid? groupId = null, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, posts) = await _postService.GetMyPendingPostsAsync(userId, groupId, skip, take);

                return status switch
                {
                    GetMyPendingPostsEnum.Success => Ok(new GetPendingPostsResponse { Message = status.GetMessage(), Posts = posts, TotalCount = posts?.Count ?? 0 }),
                    GetMyPendingPostsEnum.NoPendingPosts => Ok(new GetPendingPostsResponse { Message = status.GetMessage(), Posts = new List<PostDto>(), TotalCount = 0 }),
                    GetMyPendingPostsEnum.Failed => StatusCode(500, new GetPendingPostsResponse { Message = status.GetMessage(), Posts = null, TotalCount = 0 }),
                    _ => StatusCode(500, new GetPendingPostsResponse { Message = "Unknown error occurred", Posts = null, TotalCount = 0 })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetPendingPostsResponse
                {
                    Message = ex.Message,
                    Posts = null,
                    TotalCount = 0
                });
            }
        }
     
        [Authorize]
        [HttpPost("{postId}/cancel")]
        public async Task<IActionResult> CancelPendingPost(Guid postId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _postService.CancelPendingPostAsync(postId, userId);

                return status switch
                {
                    CancelPendingPostEnum.Success => Ok(new CancelPendingPostResponse { Message = status.GetMessage() }),
                    CancelPendingPostEnum.PostNotFound => NotFound(new CancelPendingPostResponse { Message = status.GetMessage() }),
                    CancelPendingPostEnum.PostNotPending => BadRequest(new CancelPendingPostResponse { Message = status.GetMessage() }),
                    CancelPendingPostEnum.Unauthorized => StatusCode(403, new CancelPendingPostResponse { Message = status.GetMessage() }),
                    CancelPendingPostEnum.Failed => StatusCode(500, new CancelPendingPostResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new CancelPendingPostResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new CancelPendingPostResponse { Message = ex.Message });
            }
        }
    }
}