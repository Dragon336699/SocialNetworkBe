using Domain.Contracts.Requests.FriendRequest;
using Domain.Contracts.Responses.FriendRequest;
using Domain.Enum.FriendRequest.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/friend-request")]
    public class FriendRequestController : ControllerBase
    {
        private readonly IFriendRequestService _friendRequestService;

        public FriendRequestController(IFriendRequestService friendRequestService)
        {
            _friendRequestService = friendRequestService;
        }

        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequestRequest request)
        {
            try
            {
                var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, friendRequestDto) = await _friendRequestService.SendFriendRequestAsync(request, senderId);

                return status switch
                {
                    SendFriendRequestEnum.SenderNotFound => BadRequest(new SendFriendRequestResponse { Message = status.GetMessage() }),
                    SendFriendRequestEnum.ReceiverNotFound => BadRequest(new SendFriendRequestResponse { Message = status.GetMessage() }),
                    SendFriendRequestEnum.RequestAlreadyExists => BadRequest(new SendFriendRequestResponse { Message = status.GetMessage() }),
                    SendFriendRequestEnum.AlreadyFriends => BadRequest(new SendFriendRequestResponse { Message = status.GetMessage() }),
                    SendFriendRequestEnum.CannotSendToSelf => BadRequest(new SendFriendRequestResponse { Message = status.GetMessage() }),
                    SendFriendRequestEnum.ReceiverBlocked => BadRequest(new SendFriendRequestResponse { Message = status.GetMessage() }),
                    SendFriendRequestEnum.SendFriendRequestSuccess => Ok(new SendFriendRequestResponse
                    {
                        Message = status.GetMessage(),
                        FriendRequest = friendRequestDto
                    }),
                    _ => StatusCode(500, new SendFriendRequestResponse { Message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new SendFriendRequestResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelFriendRequest([FromBody] SendFriendRequestRequest receiverId)
        {
            try
            {
                var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var success = await _friendRequestService.CancelFriendRequestAsync(senderId, receiverId);

                if (!success)
                    return BadRequest(new { Message = "Friend request not found or already handled." });

                return Ok(new { Message = "Friend request canceled successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("sent/{userId}")]
        public async Task<IActionResult> GetSentFriendRequests(Guid userId, int skip = 0, int take = 10)
        {
            try
            {
                var (status, requests) = await _friendRequestService.GetSentFriendRequestsAsync(userId, skip, take);

                if (status == GetFriendRequestsEnum.NoRequestsFound)
                    return NotFound("No sent friend requests found.");
                if (status == GetFriendRequestsEnum.SenderNotFound)
                    return BadRequest("Sender not found.");

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespondFriendRequestResponse { Message = ex.Message });
            }
        }


        [Authorize]
        [HttpPost("respond")]
        public async Task<IActionResult> RespondFriendRequest([FromBody] RespondFriendRequestRequest request)
        {
            try
            {
                var receiverId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var (status, friendRequestDto) = await _friendRequestService.RespondFriendRequestAsync(request, receiverId);

                return status switch
                {
                    RespondFriendRequestEnum.FriendRequestNotFound => NotFound(new RespondFriendRequestResponse { Message = status.GetMessage() }),
                    RespondFriendRequestEnum.Unauthorized => Forbid(),
                    RespondFriendRequestEnum.InvalidStatus => BadRequest(new RespondFriendRequestResponse { Message = status.GetMessage() }),
                    RespondFriendRequestEnum.AlreadyProcessed => BadRequest(new RespondFriendRequestResponse { Message = status.GetMessage() }),
                    RespondFriendRequestEnum.RespondFriendRequestSuccess => Ok(new RespondFriendRequestResponse
                    {
                        Message = status.GetMessage(),
                        FriendRequest = friendRequestDto
                    }),
                    _ => StatusCode(500, new RespondFriendRequestResponse { Message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespondFriendRequestResponse { Message = ex.Message });
            }
        }
 
        
    }
}