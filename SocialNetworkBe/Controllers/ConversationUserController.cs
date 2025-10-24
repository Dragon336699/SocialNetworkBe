using Domain.Contracts.Requests.ConversationUser;
using Domain.Entities;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/conversation/user/")]
    public class ConversationUserController : Controller
    {
        private readonly IConversationUserService _conversationUserService;
        public ConversationUserController(IConversationUserService conversationUserService)
        {
            _conversationUserService = conversationUserService;
        }

        [Authorize]
        [HttpPost]
        [Route("getConversationUser")]
        public async Task<IActionResult> GetConversationUser([FromBody] GetConversationUserRequest request)
        {
            try
            {
                IEnumerable<ConversationUser>? conversationUser = await _conversationUserService.GetConversationUser(request);
                if (conversationUser == null) return BadRequest(new { message = "Conversation doesn't exist!" });
                return Ok(new { message = "Get conversation user successfully", data = conversationUser });
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
