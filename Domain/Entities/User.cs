using Domain.Enum.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public required String FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public required UserStatus Status {  get; set; }
    }
}
