using Domain.Contracts.Requests.Conversation;
using Domain.Contracts.Responses.Conversation;
using Domain.Contracts.Responses.Post;
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
        [Route("create")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
        {
            try
            {              
                var senderIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(senderIdClaim, out var senderId))
                {
                    return Unauthorized(new CreateConversationResponse { Message = "Invalid token." });
                }
                var (status, conversationId) = await _conversationService.CreateConversationAsync(senderId, request.ReceiverUserName);

                return status switch
                {                  
                    CreateConversationEnum.ReceiverNotFound => BadRequest(new CreateConversationResponse{Message = status.GetMessage()}),
                    CreateConversationEnum.ConversationExists => Ok(new CreateConversationResponse{ConversationId = conversationId,Message = status.GetMessage()}),
                    CreateConversationEnum.CreateConversationSuccess => Ok(new CreateConversationResponse{ConversationId = conversationId,Message = status.GetMessage()}),
                    _ => StatusCode(500, new CreateConversationResponse{Message = status.GetMessage()})
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }
    }
}