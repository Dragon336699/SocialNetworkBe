using Domain.Contracts.Requests.Conversation;
using Domain.Enum.Conversation;
using Domain.Enum.Conversation.Functions;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkBe.Services;
using System;
using System.Threading.Tasks;

namespace SocialNetworkBe.Controllers
{
    [Route("api/v1")]  
    [ApiController]  
    public class ConversationController : Controller
    {
        private readonly ConversationService _conversationService;

        public ConversationController(ConversationService conversationService)
        {        
            _conversationService = conversationService;
        }

        [HttpPost]
        [Route("conversation")] 
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
        {
            try
            {
                // Kiểm tra request không null và User1Id/User2Id/Type hợp lệ
                if (request == null || request.User1Id == Guid.Empty || request.User2Id == Guid.Empty || request.Type == 0)
                {
                    return BadRequest(new { message = "Invalid input: user1Id, user2Id, and type are required." });
                }

                // Kiểm tra nếu User1Id và User2Id giống nhau
                if (request.User1Id == request.User2Id)
                {
                    return BadRequest(new { message = "Cannot create conversation with yourself." });
                }

             
                var result = await _conversationService.CreateConversationAsync(request.User1Id, request.User2Id, request.Type);
                return result switch
                {                  
                    ConversationStatus.CreatedSuccessfully => Ok(new { message = result.GetMessage() }),                
                    ConversationStatus.AlreadyExists => BadRequest(new { message = result.GetMessage() }),                   
                    ConversationStatus.InvalidInput => BadRequest(new { message = result.GetMessage() }),                
                    _ => StatusCode(500, new { message = result.GetMessage() })
                };
            }
            catch (Exception ex)
            {              
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}