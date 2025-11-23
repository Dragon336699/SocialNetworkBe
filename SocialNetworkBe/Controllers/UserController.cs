using Azure.Core;
using Domain.Contracts.Requests.User;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.User.Functions;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkBe.Services.OTPServices;
using SocialNetworkBe.Services.UserServices;
using System.Security.Claims;

namespace SocialNetworkBe.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
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
            return confirmationResult switch
            {
                ConfirmationEmailEnum.UserNotFound => Redirect("http://localhost:3000/confirmEmail/userNotFound"),
                ConfirmationEmailEnum.Invalid => Redirect("http://localhost:3000/confirmEmail/invalid"),
                ConfirmationEmailEnum.Fail => Redirect("http://localhost:3000/confirmEmail/fail"),
                ConfirmationEmailEnum.Success => Redirect("http://localhost:3000/confirmEmail/success"),
                _ => StatusCode(500, new { message = confirmationResult.ToString() })
            };
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
                    LoginEnum.LoginSucceded => HandleLoginSuccess(result.jwtValue, loginResult),
                    _ => StatusCode(500, new { message = loginResult.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {message = ex.Message});
            }
        }

        [Authorize]
        [HttpPost]
        [Route("user/logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized(new { message = "User not authenticated" });
             
                Response.Cookies.Delete("jwt", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
               
                if (Guid.TryParse(userId, out Guid userGuid))
                {
                    await _userService.UpdateUserStatus(userGuid, Domain.Enum.User.Types.UserStatus.Offline);
                }

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private IActionResult HandleLoginSuccess(string token, LoginEnum loginResult)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(30)
            };

            Response.Cookies.Append("jwt", token, cookieOptions);

            return Ok(new { message = loginResult.GetMessage() });
        }

        [Authorize]
        [HttpPost]
        [Route("user/changePassword")]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return BadRequest(new { message = "User not found!" });
                var changePasswordResult = await _userService.ChangePassword(request, userId);

                return changePasswordResult switch
                {
                    ChangePasswordEnum.OldPasswordIncorrect => BadRequest(new { message = changePasswordResult.GetMessage() }),
                    ChangePasswordEnum.UserNotFound => BadRequest(new { message = changePasswordResult.GetMessage() }),
                    ChangePasswordEnum.DuplicatePassword => BadRequest(new { message = changePasswordResult.GetMessage() }),
                    ChangePasswordEnum.ChangePasswordSuccess => Ok(new { message = changePasswordResult.GetMessage() }),
                    _ => StatusCode(500, new { message = changePasswordResult.GetMessage() })
                };
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("user/forgetPassword/getOTP")]
        public async Task<IActionResult> GetOTP(string email)
        {
            try
            {
                var (status, otp) = await _userService.GetOTP(email);
                return status switch
                {
                    GetOTPEnum.UserNotFound => BadRequest(new { message = status.GetMessage() }),
                    GetOTPEnum.SpamOTP => BadRequest(new { message = status.GetMessage() }),
                    GetOTPEnum.SentOTP => Ok(new { message = status.GetMessage() }),
                    _ => StatusCode(500, new { message = status.GetMessage() })
                };
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("user/forgetPassword/validateOTP")]
        public async Task<IActionResult> ValidateOTP([FromBody] ValidateOTPRequest request)
        {
            try
            {
                var (validateOTPStatus, resetPasswordToken) = await _userService.ValidateOTP(request);
                return validateOTPStatus switch
                {
                    ValidateOTPEnum.UserNotFound => BadRequest(new { message = validateOTPStatus.GetMessage() }),
                    ValidateOTPEnum.IncorrectOTP => BadRequest(new { message = validateOTPStatus.GetMessage() }),
                    ValidateOTPEnum.CorrectOTP => Ok(new { message = validateOTPStatus.GetMessage(), resetPasswordToken = resetPasswordToken }),
                    _ => StatusCode(500, new { message = validateOTPStatus.GetMessage() })
                };
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("user/forgetPassword/resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var resetPasswordStatus = await _userService.ResetPassword(request);
                return resetPasswordStatus switch
                {
                    ResetPasswordEnum.UserNotFound => BadRequest(new { message = resetPasswordStatus.GetMessage() }),
                    ResetPasswordEnum.ResetPasswordFail => BadRequest(new { message = resetPasswordStatus.GetMessage() }),
                    ResetPasswordEnum.ResetPasswordSuccess => Ok(new { message = resetPasswordStatus.GetMessage() }),
                    _ => StatusCode(500, new { message = resetPasswordStatus.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("user/googleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                LoginRes result = await _userService.GoogleLogin(request.GoogleToken);
                LoginEnum loginResult = result.loginResult;
                return loginResult switch
                {
                    LoginEnum.LoginFailed => BadRequest(new { message = loginResult.GetMessage() }),
                    LoginEnum.EmailUnConfirmed => BadRequest(new { message = loginResult.GetMessage() }),
                    LoginEnum.LoginSucceded => HandleLoginSuccess(result.jwtValue, loginResult),
                    _ => StatusCode(500, new { message = loginResult.GetMessage() })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("user/getUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return BadRequest(new { message = "User Not Found" });
                var user = await _userService.GetUserInfoByUserId(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("user/getUserInfoByUserName")]
        public async Task<IActionResult> GetUserInfoByUserName([FromQuery] string userName)
        {
            try
            {
                var (status, user) = await _userService.GetUserInfoByUserName(userName);
                if (!status) return BadRequest(new { message = "User Not Found" });
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("user/getUserInfoById")]
        public async Task<IActionResult> GetUserInfoById([FromQuery] string userId)
        {
            try
            {
                var user = await _userService.GetUserInfoByUserId(userId);
                if (user == null) return BadRequest(new { message = "User Not Found" });
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("user/search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string keyword)
        {
            try
            {
                var users = await _userService.SearchUser(keyword);
                if (users == null) return BadRequest(new { message = "Users Not Found" });
                return Ok(new { data = users, message = "Search users successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("user/changeAvatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] UpdateAvatarRequest request)
        {

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return BadRequest(new { message = "User Not Found" });
                var (status, avatarUrl) = await _userService.UpdateAvatarAsync(request, userId);
                var message = status.GetMessage();

                return status switch
                {
                    UpdateAvatarEnum.Success =>
                        Ok(new { message }),

                    UpdateAvatarEnum.UserNotFound =>
                        NotFound(new { message }),

                    UpdateAvatarEnum.InvalidImageFormat =>
                        BadRequest(new { message }),

                    UpdateAvatarEnum.FileTooLarge =>
                        BadRequest(new { message }),

                    UpdateAvatarEnum.UploadFailed =>
                        StatusCode(500, new { message }),

                    _ =>
                        StatusCode(500, new { message = "Unexpected error occurred." })
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("user/updateInfo")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BadRequest(new { message = "User not found" });

            var status = await _userService.UpdateUserInfoAsync(request, userId);

            return status switch
            {
                UpdateUserInfoEnum.Success => Ok(new { message = status.GetMessage() }),
                UpdateUserInfoEnum.UserNotFound => NotFound(new { message = status.GetMessage() }),
                UpdateUserInfoEnum.InvalidEmail => BadRequest(new { message = status.GetMessage() }),
                UpdateUserInfoEnum.UploadFailed => StatusCode(500, new { message = status.GetMessage() }),
                _ => StatusCode(500, new { message = status.GetMessage() })
            };
        }
    }
}
