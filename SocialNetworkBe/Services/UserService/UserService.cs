using AutoMapper;
using Domain.Contracts.Requests;
using Domain.Entities;
using Domain.Enum.Role.Functions;
using Domain.Enum.User;
using Domain.Enum.User.Functions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;

namespace SocialNetworkBe.Services.UserService
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<UserService> _logger;
        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper, IEmailSender emailSender , ILogger<UserService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _logger = logger;
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
                return (false, UserRegistrationEnum.RegistrationFailure.GetMessage());
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
    }
}
