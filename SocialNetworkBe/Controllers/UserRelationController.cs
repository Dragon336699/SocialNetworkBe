using Domain.Contracts.Requests.UserRelation;
using Domain.Entities;
using Domain.Enum.UserRelation.Funtions;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/user-relation")]
    public class UserRelationController : ControllerBase
    {
        private readonly IUserRelationService _userRelationService;

        public UserRelationController(IUserRelationService userRelationService)
        {
            _userRelationService = userRelationService;
        }

        [Authorize]
        [HttpPost("follow")]
        public async Task<IActionResult> FollowUser([FromBody] FollowUserRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var status = await _userRelationService.FollowUserAsync(currentUserId, request.TargetUserId);

                return status switch
                {
                    FollowUserEnum.Success => Ok(new { Message = status.GetMessage() }),
                    FollowUserEnum.TargetUserNotFound => NotFound(new { Message = status.GetMessage() }),
                    FollowUserEnum.AlreadyFollowing => BadRequest(new { Message = status.GetMessage() }),
                    FollowUserEnum.CannotFollowSelf => BadRequest(new { Message = status.GetMessage() }),
                    _ => StatusCode(500, new { Message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("unfollow")]
        public async Task<IActionResult> UnfollowUser([FromBody] FollowUserRequest request)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var status = await _userRelationService.UnfollowUserAsync(currentUserId, request.TargetUserId);

                return status switch
                {
                    UnfollowUserEnum.Success => Ok(new { Message = status.GetMessage() }),
                    UnfollowUserEnum.NotFollowing => BadRequest(new { Message = status.GetMessage() }),
                    UnfollowUserEnum.TargetUserNotFound => NotFound(new { Message = "User not found." }),
                    _ => StatusCode(500, new { Message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowers([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _userRelationService.GetFollowersAsync(userId, pageIndex, pageSize);
                return Ok(new { Message = "Get followers successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("following")]
        public async Task<IActionResult> GetFollowing([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _userRelationService.GetFollowingAsync(userId, pageIndex, pageSize);
                return Ok(new { Message = "Get following list successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriends([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _userRelationService.GetFriendsAsync(userId, skip, take);
                return Ok(new { Message = "Get friends list successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
