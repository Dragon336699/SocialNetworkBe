using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.User
{
    public class LoginRequest
    {
        [Required]
        public required string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
