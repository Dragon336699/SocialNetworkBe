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
    }
}