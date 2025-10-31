using Domain.Contracts.Requests.User;
using Domain.Contracts.Responses.User;
using Domain.Enum.User.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ServiceInterfaces
{
    public interface IUserService
    {
        Task<(bool, string)> UserRegisterAsync(UserRegistrationRequest request, string baseUrl);
        Task<ConfirmationEmailEnum> ConfirmationEmail(string token, string email);
        Task<LoginRes> UserLogin(Domain.Contracts.Requests.User.LoginRequest loginRequest);
        Task<ChangePasswordEnum> ChangePassword(ChangePasswordRequest request, string userId);
        Task<(GetOTPEnum, string?)> GetOTP(string email);
        Task<(ValidateOTPEnum, string?)> ValidateOTP(ValidateOTPRequest request);
        Task<ResetPasswordEnum> ResetPassword(ResetPasswordRequest request);
        Task<LoginRes> GoogleLogin(string googleToken);
        Task<UserDto?> GetUserInfoByUserId(string userId);
        Task<(bool, UserDto?)> GetUserInfoByUserName(string userName);
        Task<IEnumerable<UserDto>?> SearchUser(string keyword);
    }
}
