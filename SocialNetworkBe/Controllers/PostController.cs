using Domain.Contracts.Requests.Post;
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

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request) 
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));              
                var (status, postId) = await _postService.CreatePostAsync(request, userId);

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
                        PostId = postId
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
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPosts([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var (status, posts) = await _postService.GetAllPostsAsync(skip, take);

                return status switch
                {
                    GetAllPostsEnum.Success => Ok(new GetAllPostsResponse
                    {
                        Message = status.GetMessage(),
                        Posts = posts,
                        TotalCount = posts?.Count ?? 0
                    }),
                    GetAllPostsEnum.NoPostsFound => Ok(new GetAllPostsResponse
                    {
                        Message = status.GetMessage(),
                        Posts = new List<PostDto>(),
                        TotalCount = 0
                    }),
                    GetAllPostsEnum.Failed => BadRequest(new GetAllPostsResponse
                    {
                        Message = status.GetMessage(),
                        Posts = null,
                        TotalCount = 0
                    }),
                    _ => StatusCode(500, new GetAllPostsResponse
                    {
                        Message = "Unknown error occurred",
                        Posts = null,
                        TotalCount = 0
                    })
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
    }
}