using Domain.Contracts.Requests.User;
using Domain.Enum.User.Functions;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace SocialNetworkBe.Services.OTPServices
{
    public class OTPService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<OTPService> _logger;
        public OTPService(IMemoryCache memoryCache, ILogger<OTPService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public (GetOTPEnum, string?) GenerateAndStoreOTP(string userEmail)
        {
            try
            {
                var otpExpiry = TimeSpan.FromMinutes(3);
                int maxRequestPerHour = 5;
                string otpRequestEmail = $"OTP_Request_{userEmail}";
                if (!_memoryCache.TryGetValue(otpRequestEmail, out int value))
                {
                    value = 0;
                }

                if (value > maxRequestPerHour) return (GetOTPEnum.SpamOTP, null);

                string otp = GenerateSecureOTP();
                string setOtpEmail = $"OTP_Stored_{userEmail}";

                _memoryCache.Set(otpRequestEmail, value + 1, TimeSpan.FromMinutes(30));
                _memoryCache.Set(setOtpEmail, otp, otpExpiry);
                return (GetOTPEnum.SentOTP, otp);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating OTP");
                throw;
            }
        }

        private string GenerateSecureOTP(int length = 6)
        {
            const string digits = "0123456789";
            byte[] data = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }
            char[] otp = new char[length];
            for (int i = 0; i < length; i++)
            {
                otp[i] = digits[data[i] % digits.Length];
            }

            return new string(otp);
        }

        public bool ValidateOTP(ValidateOTPRequest request)
        {
            string storedOTPEmail = $"OTP_Stored_{request.Email}";
            string otpRequestEmail = $"OTP_Request_{request.Email}";
            if (_memoryCache.TryGetValue(storedOTPEmail, out string? storedOTP) && storedOTP == request.OTP)
            {
                _memoryCache.Remove(storedOTPEmail);
                _memoryCache.Remove(otpRequestEmail);
                return true;
            }
            return false;
        }
    }
}
