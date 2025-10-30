using Domain.Contracts.Requests.Message;
using Domain.Contracts.Responses.Message;
using Domain.Entities;
using Domain.Enum.Message.Functions;
using Domain.Enum.User.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/message/")]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [Authorize]
        [HttpPost]
        [Route("getMessages")]
        public async Task<IActionResult> GetMessages([FromBody] GetMessagesRequest request)
        {
            try
            {
                var (status, messages) = await _messageService.GetMessages(request);
                return status switch
                {
                    GetMessagesEnum.UserNotFound => BadRequest(new { message = status.GetMessage() }),
                    GetMessagesEnum.GetMessageFail => BadRequest(new { message = status.GetMessage() }),
                    GetMessagesEnum.GetMessageSuccess => Ok(new { data = messages, message = status.GetMessage() }),
                    _ => StatusCode(500, new { message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("sendMessage")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageRequest request)
        {
            try
            {
                var (status, newMessage) = await _messageService.SendMessage(request);
                return status switch
                {
                    SendMessageEnum.SenderNotFound => BadRequest(new { message = status.GetMessage() }),
                    SendMessageEnum.SendMessageFailed => BadRequest(new { message = status.GetMessage() }),
                    SendMessageEnum.UploadImageFailed => BadRequest(new { message = status.GetMessage() }),
                    SendMessageEnum.SendMessageSuceeded => Ok(new { data = newMessage, message = status.GetMessage() }),
                    _ => StatusCode(500, new { message = status.GetMessage() })
                };
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("reaction")]
        public async Task<IActionResult> AddUpdateDeleteReactionMessage([FromBody] ReactionMessageRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();
                var newMessage = await _messageService.AddUpdateDeleteReactionMessage(request, Guid.Parse(userId));
                if (newMessage == null) return BadRequest(new { message = "Reaction failed" });
                return Ok(new { data = newMessage, message = "Reaction successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
