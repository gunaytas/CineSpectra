using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineSpectra.Application.DTOs
{
    public class RegisterDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
    }

    public class LoginDto
    {
        public string UserNameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public List<string> Roles { get; set; } = new();
        public string? Message { get; set; }
    }
}