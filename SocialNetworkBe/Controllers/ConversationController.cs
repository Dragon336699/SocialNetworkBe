using Domain.Contracts.Requests.Conversation;
using Domain.Contracts.Responses.Conversation;
using Domain.Contracts.Responses.Post;
using Domain.Entities;
using Domain.Enum.Conversation.Functions;
using Domain.Enum.Message.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkBe.Services.MessageService;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/conversation/")]
    public class ConversationController : Controller
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [Authorize]
        [HttpPost]
        [Route("createConversation")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
        {
            try
            {
                var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));               
                if (!request.UserIds.Contains(senderId))
                {
                    request.UserIds.Add(senderId);
                }

                var (status, conversationId) = await _conversationService.CreateConversationAsync(request.ConversationType, request.UserIds);

                return status switch              
                {
                CreateConversationEnum.ReceiverNotFound => BadRequest(new { Message = status.GetMessage() }),
                    CreateConversationEnum.ConversationExists => Ok(new { data = conversationId, Message = status.GetMessage() }),
                    CreateConversationEnum.CreateConversationSuccess => Ok(new { data = conversationId, Message = status.GetMessage() }),
                    _ => StatusCode(500, new { Message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }


        [Authorize]
        [HttpGet]
        [Route("getConversation")]
        public async Task<IActionResult> GetConversationById(Guid conversationId)
        {
            try
            {
                var conversation = await _conversationService.GetConversationById(conversationId);
                if (conversation == null) return BadRequest(new { message = "Conversation doesn't exist!" });
                return Ok(new {message = "Get conversation successfully", data =  conversation});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getAllConversationsByUser")]
        public async Task<IActionResult> GetAllConversationByUser()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                List<ConversationDto>? conversations = await _conversationService.GetAllConversationByUser(userId);
                if (conversations == null) return BadRequest(new { message = "Conversations doesn't exist!" });
                return Ok(new { message = "Get conversations successfully", data = conversations });
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("deleteConversation")]
        public async Task<IActionResult> DeleteConversation([FromBody] DeleteConversationRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var status = await _conversationService.DeleteConversationAsync(request.ConversationId, userId);

                return status switch
                {
                    DeleteConversationEnum.Success => Ok(new { message = status.GetMessage() }),
                    DeleteConversationEnum.ConversationNotFound => NotFound(new { message = status.GetMessage() }),
                    DeleteConversationEnum.UserNotInConversation => Forbid(),
                    _ => StatusCode(500, new { message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("changeNickname")]
        public async Task<IActionResult> ChangeNickname([FromBody] ChangeNicknameRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var status = await _conversationService.ChangeNicknameAsync(
                    request.ConversationId,
                    userId,
                    request.TargetUserId,
                    request.NewNickname
                );

                return status switch
                {
                    ChangeNicknameEnum.Success => Ok(new { message = status.GetMessage() }),
                    ChangeNicknameEnum.ConversationNotFound => NotFound(new { message = status.GetMessage() }),
                    ChangeNicknameEnum.UserNotInConversation => BadRequest(new { message = status.GetMessage() }),
                    _ => StatusCode(500, new { message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("changeConversationName")]
        public async Task<IActionResult> ChangeConversationName([FromBody] ChangeConversationNameRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var status = await _conversationService.ChangeConversationNameAsync(
                    request.ConversationId,
                    userId,
                    request.NewConversationName
                );

                return status switch
                {
                    ChangeConversationNameEnum.Success => Ok(new { message = status.GetMessage() }),
                    ChangeConversationNameEnum.ConversationNotFound => NotFound(new { message = status.GetMessage() }),
                    ChangeConversationNameEnum.UserNotInConversation => Forbid(),
                    ChangeConversationNameEnum.NotGroupConversation => BadRequest(new { message = status.GetMessage() }),
                    _ => StatusCode(500, new { message = status.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Bỏ rồi
        [Authorize]
        [HttpPost]
        [Route("getConversationForList")]
        public async Task<IActionResult> GetConversationForList([FromBody] Guid conversationId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ConversationDto? conversation = await _conversationService.GetConversationForList(conversationId, userId);
                if (conversation == null) return BadRequest(new { message = "Conversations doesn't exist!" });
                return Ok(new { message = "Get conversations successfully", data = conversation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}