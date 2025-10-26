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
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request) // ✅ Đổi từ [FromBody] thành [FromForm]
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
    }
}