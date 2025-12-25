using Domain.Contracts.Requests.Interaction;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/interaction")]
    public class InteractionController : ControllerBase
    {
        private readonly IInteractionService _interactionService;
        public InteractionController(IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        [Authorize]
        [HttpPost("search")]
        public IActionResult IncreaseSearch([FromBody] InteractionUserRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _interactionService.IncreaseSearchCount(userId, request.TargetUserId);

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("view")]
        public IActionResult IncreaseView([FromBody] InteractionUserRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _interactionService.IncreaseViewCount(userId, request.TargetUserId);

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("like")]
        public IActionResult IncreaseLike([FromBody] InteractionUserRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _interactionService.IncreaseLikeCount(userId, request.TargetUserId);

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
