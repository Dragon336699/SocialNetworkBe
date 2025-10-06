using AutoMapper;
using Domain.Contracts.Requests.User;
using Domain.Contracts.Responses.User;
using Domain.Entities;
using Domain.Enum.Role.Functions;
using Domain.Enum.User;
using Domain.Enum.User.Functions;
using Domain.Interfaces.UnitOfWorkInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using SocialNetworkBe.Services.TokenServices;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace SocialNetworkBe.Services.UserServices
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<UserService> _logger;
        private readonly TokenService _tokenService;
        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper, IEmailSender emailSender , ILogger<UserService> logger, TokenService tokenService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _logger = logger;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool, string)> UserRegisterAsync(UserRegistrationRequest request, string baseUrl)
        {
            try
            {
                var user = _mapper.Map<User>(request);

                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    var role = new Role { Name = "User" };
                    var createRoleResult = await _roleManager.CreateAsync(role);
                    if (!createRoleResult.Succeeded)
                    {
                        return (false, CreateRoleReturn.CreateRoleFailure.GetMessage());
                    }
                }

                var createUserResult = await _userManager.CreateAsync(user, request.Password);
                if (!createUserResult.Succeeded) return (false, UserRegistrationEnum.CreatedUserFailure.GetMessage());

                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string encodedToken = WebUtility.UrlEncode(token);
                var confirmationLink = $"{baseUrl}/api/v1/confirmationEmail?token={encodedToken}&email={request.Email}";

                await _emailSender.SendEmailAsync(request.Email, "Confirmation Email Link",
                    $@"
                            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd;'>
                            <h2 style='color: #333;'>Hello</h2>
                            <p style='margin-top: 10px;'>Thank you for signing up. Please confirm your email by clicking the button below:</p>
                            <div style='margin-top: 20px;'>
                                <a href='{confirmationLink}' style='background-color: #007BFF; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Confirm Email</a>
                            </div>
                            <p style='margin-top: 20px; font-size: 12px; color: #999;'>This email was sent by FriCon. If you didn't sign up, you can safely ignore this email.</p>
                        </div>
                        "
                    );

                await _userManager.AddToRoleAsync(user, "User");
                return (true, UserRegistrationEnum.RegistrationSuccess.GetMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration reason fail with email {Email}", request.Email);
                throw;
            }
        }

        public async Task<ConfirmationEmailEnum> ConfirmationEmail(string token, string email)
        {
            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(email)) return ConfirmationEmailEnum.Invalid;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return ConfirmationEmailEnum.UserNotFound;

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded) return ConfirmationEmailEnum.Success;

            return ConfirmationEmailEnum.Fail;
        }

        public async Task<LoginRes> UserLogin(Domain.Contracts.Requests.User.LoginRequest loginRequest)
        {
            try
            {
                LoginRes returnResult = new LoginRes
                {
                    loginResult = LoginEnum.LoginFailed,
                    jwtValue = null,
                };

                var user = await _userManager.FindByEmailAsync(loginRequest.Email);

                if (user == null) return returnResult;

                if (!user.EmailConfirmed)
                {
                    returnResult.loginResult = LoginEnum.EmailUnConfirmed;
                    return returnResult;
                };

                if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, loginRequest.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Định danh duy nhất cho token
                };

                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var jwtValue = _tokenService.GenerateJwt(authClaims);
                    returnResult.jwtValue = new JwtSecurityTokenHandler().WriteToken(jwtValue);
                    returnResult.loginResult = LoginEnum.LoginSucceded;
                    return returnResult;
                }
            
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error login by user with email: ${Email}", loginRequest.Email);
                throw;
            }
            return null;
        }

        public async Task<ChangePasswordEnum> ChangePassword (ChangePasswordRequest request, string userId)
        {
            try
            {
                if (request.OldPassword == request.NewPassword) return ChangePasswordEnum.DuplicatePassword;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return ChangePasswordEnum.UserNotFound;

                var changePassRes = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
                if (!changePassRes.Succeeded) return ChangePasswordEnum.OldPasswordIncorrect;
                return ChangePasswordEnum.ChangePasswordSuccess;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error while changing password");
                throw;
            }
        }

    }
}
