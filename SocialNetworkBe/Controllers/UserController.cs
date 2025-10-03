using Domain.Contracts.Requests.User;
using Domain.Contracts.Responses.User;
using Domain.Enum.User.Functions;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkBe.Services.UserServices;

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
            try
            {
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                var (isSuccess, message) = await _userService.UserRegisterAsync(userRegistrationRequest, baseUrl);
                if (!isSuccess) return BadRequest(new { message = message });
                return Ok(new { message = message });
            } catch (Exception ex)
            {
                return StatusCode(500, new {message = ex.Message});
            }
            
        }

        [HttpGet]
        [Route("user/confirmationEmail")]
        public async Task<IActionResult> ConfirmationEmail (string token, string email)
        {
            ConfirmationEmailEnum confirmationResult = await _userService.ConfirmationEmail(token, email);
            return Ok(confirmationResult);
        }

        [HttpPost]
        [Route("user/login")]
        public async Task<IActionResult> UserLogin([FromBody] LoginRequest request)
        {
            try
            {
                LoginRes result = await _userService.UserLogin(request);
                LoginEnum loginResult = result.loginResult;
                return loginResult switch
                {
                    LoginEnum.LoginFailed => BadRequest(new { message = loginResult.GetMessage() }),
                    LoginEnum.EmailUnConfirmed => BadRequest(new { message = loginResult.GetMessage() }),
                    LoginEnum.LoginSucceded => Ok(new { message = loginResult.GetMessage(), token = result.jwtValue }),
                    _ => StatusCode(500, new { message = loginResult.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {message = ex.Message});
            }
        }
    }
}
