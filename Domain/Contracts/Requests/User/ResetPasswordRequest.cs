using System.ComponentModel.DataAnnotations;

namespace Domain.Contracts.Requests.User
{
    public class ResetPasswordRequest
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string ResetPasswordToken { get; set; }
        [Required]
        public required string NewPassword { get; set; }
    }
}
