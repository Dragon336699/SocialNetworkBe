using Domain.Enum.Conversation.Functions;
using Domain.Contracts.Requests.Conversation;
using SocialNetworkBe.Services;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateConversation([FromBody] ConversationRequest request)
        {
            try
            {
                // Kiểm tra request không null và các trường hợp hợp lệ
                if (request == null || request.SenderId == Guid.Empty || request.ReceiverId == Guid.Empty ||
                    string.IsNullOrEmpty(request.Content) || request.Type == 0)
                {
                    return BadRequest(new { message = "Invalid input: senderId, receiverId, content, and type are required." });
                }

                // Kiểm tra nếu SenderId và ReceiverId giống nhau
                if (request.SenderId == request.ReceiverId)
                {
                    return BadRequest(new { message = "Cannot create conversation with yourself." });
                }

                // Gọi dịch vụ để tạo cuộc hội thoại và gửi tin nhắn
                var result = await _conversationService.CreateConversationAsync(request.SenderId, request.ReceiverId, request.Content, request.Type);
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