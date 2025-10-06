using System.ComponentModel.DataAnnotations;

namespace Domain.Contracts.Requests.User
{
    public class ChangePasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public required string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }
    }
}
