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
        [Authorize]
        [HttpGet("sent")]
        public async Task<IActionResult> GetSentFriendRequests([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Gọi service
                var result = await _friendRequestService.GetSentFriendRequestsAsync(senderId, pageIndex, pageSize);

                // Trả về kết quả
                return Ok(new
                {
                    Message = "Get sent friend requests successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelFriendRequest([FromBody] CancelFriendRequestRequest request)
        {
            try
            {
                var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var status = await _friendRequestService.CancelFriendRequestAsync(request, senderId);

                return status switch
                {
                    CancelFriendRequestEnum.RequestNotFound => NotFound(new { Message = status.GetMessage() }),
                    CancelFriendRequestEnum.NotPending => BadRequest(new { Message = status.GetMessage() }),
                    CancelFriendRequestEnum.Success => Ok(new { Message = status.GetMessage() }),
                    _ => StatusCode(500, new { Message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}