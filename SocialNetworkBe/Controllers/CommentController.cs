using Domain.Contracts.Requests.Comment;
using Domain.Contracts.Responses.Comment;
using Domain.Enum.Comment.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/comment")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateComment([FromForm] CreateCommentRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, commentId) = await _commentService.CreateCommentAsync(request, userId);

                return status switch
                {
                    CreateCommentEnum.UserNotFound => BadRequest(new CreateCommentResponse { Message = status.GetMessage() }),
                    CreateCommentEnum.PostNotFound => BadRequest(new CreateCommentResponse { Message = status.GetMessage() }),
                    CreateCommentEnum.ParentCommentNotFound => BadRequest(new CreateCommentResponse { Message = status.GetMessage() }),
                    CreateCommentEnum.InvalidContent => BadRequest(new CreateCommentResponse { Message = status.GetMessage() }),
                    CreateCommentEnum.InvalidImageFormat => BadRequest(new CreateCommentResponse { Message = status.GetMessage() }),
                    CreateCommentEnum.FileTooLarge => BadRequest(new CreateCommentResponse { Message = status.GetMessage() }),
                    CreateCommentEnum.ImageUploadFailed => BadRequest(new CreateCommentResponse { Message = status.GetMessage() }),
                    CreateCommentEnum.CreateCommentSuccess => Ok(new CreateCommentResponse
                    {
                        Message = status.GetMessage(),
                        CommentId = commentId
                    }),
                    _ => StatusCode(500, new CreateCommentResponse { Message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new CreateCommentResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(Guid postId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var (status, comments) = await _commentService.GetCommentsByPostIdAsync(postId, skip, take);

                return status switch
                {
                    GetCommentsEnum.Success => Ok(new GetCommentsResponse
                    {
                        Message = status.GetMessage(),
                        Comments = comments,
                        TotalCount = comments?.Count ?? 0
                    }),
                    GetCommentsEnum.PostNotFound => NotFound(new GetCommentsResponse
                    {
                        Message = status.GetMessage(),
                        Comments = null,
                        TotalCount = 0
                    }),
                    GetCommentsEnum.NoCommentsFound => Ok(new GetCommentsResponse
                    {
                        Message = status.GetMessage(),
                        Comments = new List<CommentDto>(),
                        TotalCount = 0
                    }),
                    GetCommentsEnum.Failed => BadRequest(new GetCommentsResponse
                    {
                        Message = status.GetMessage(),
                        Comments = null,
                        TotalCount = 0
                    }),
                    _ => StatusCode(500, new GetCommentsResponse
                    {
                        Message = "Unknown error occurred",
                        Comments = null,
                        TotalCount = 0
                    })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GetCommentsResponse
                {
                    Message = ex.Message,
                    Comments = null,
                    TotalCount = 0
                });
            }
        }

        [Authorize]
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment([FromForm] UpdateCommentRequest request, Guid commentId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, commentDto) = await _commentService.UpdateCommentAsync(commentId, request, userId);

                return status switch
                {
                    UpdateCommentEnum.CommentNotFound => NotFound(new UpdateCommentResponse { Message = status.GetMessage() }),
                    UpdateCommentEnum.Unauthorized => Forbid(),
                    UpdateCommentEnum.InvalidContent => BadRequest(new UpdateCommentResponse { Message = status.GetMessage() }),
                    UpdateCommentEnum.InvalidImageFormat => BadRequest(new UpdateCommentResponse { Message = status.GetMessage() }),
                    UpdateCommentEnum.FileTooLarge => BadRequest(new UpdateCommentResponse { Message = status.GetMessage() }),
                    UpdateCommentEnum.ImageUploadFailed => BadRequest(new UpdateCommentResponse { Message = status.GetMessage() }),
                    UpdateCommentEnum.UpdateCommentSuccess => Ok(new UpdateCommentResponse
                    {
                        Message = status.GetMessage(),
                        Comment = commentDto
                    }),
                    UpdateCommentEnum.UpdateCommentFailed => StatusCode(500, new UpdateCommentResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new UpdateCommentResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UpdateCommentResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, result) = await _commentService.DeleteCommentAsync(commentId, userId);

                return status switch
                {
                    DeleteCommentEnum.CommentNotFound => NotFound(new DeleteCommentResponse { Message = status.GetMessage() }),
                    DeleteCommentEnum.Unauthorized => Forbid(),
                    DeleteCommentEnum.DeleteCommentSuccess => Ok(new DeleteCommentResponse { Message = status.GetMessage() }),
                    DeleteCommentEnum.DeleteCommentFailed => StatusCode(500, new DeleteCommentResponse { Message = status.GetMessage() }),
                    _ => StatusCode(500, new DeleteCommentResponse { Message = "Unknown error occurred" })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DeleteCommentResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("reaction")]
        public async Task<IActionResult> AddUpdateDeleteReactionComment([FromBody] ReactionCommentRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var updatedComment = await _commentService.AddUpdateDeleteReactionComment(request, userId);

                if (updatedComment == null)
                    return BadRequest(new ReactionCommentResponse { Message = "Reaction failed" });

                return Ok(new ReactionCommentResponse { Message = "Reaction successfully", Comment = updatedComment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ReactionCommentResponse { Message = ex.Message });
            }
        }
    }
}