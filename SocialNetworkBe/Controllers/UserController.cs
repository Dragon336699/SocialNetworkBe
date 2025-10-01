using Domain.Contracts.Requests;
using Domain.Enum.User.Functions;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkBe.Services.UserService;

namespace SocialNetworkBe.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("user/register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest userRegistrationRequest)
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            var (isSuccess, message) = await _userService.UserRegisterAsync(userRegistrationRequest, baseUrl);
            if (!isSuccess) return BadRequest(new { message = message });
            return Ok(new {message = message });
        }

        [HttpGet]
        [Route("user/confirmationEmail")]
        public async Task<IActionResult> ConfirmationEmail (string token, string email)
        {
            ConfirmationEmailEnum confirmationResult = await _userService.ConfirmationEmail(token, email);
            return Ok(confirmationResult);
        }

    }
}
