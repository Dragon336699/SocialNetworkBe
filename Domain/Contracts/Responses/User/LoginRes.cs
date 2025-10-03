using Domain.Enum.User.Functions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Responses.User
{
    public class LoginRes
    {
        public LoginEnum loginResult {  get; set; } 
        public string? jwtValue { get; set; }

    }
}
