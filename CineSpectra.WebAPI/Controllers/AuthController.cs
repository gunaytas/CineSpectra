using CineSpectra.Application.DTOs;
using CineSpectra.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CineSpectra.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser != null)
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = "Bu kullanıcı adı zaten alınmış." });

            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(new AuthResponseDto { IsSuccess = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            // İlk kullanıcı otomatik Admin rolü alabilir veya varsayılan "User" atanır
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new AppRole { Name = "User" });
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new AppRole { Name = "Admin" });

            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new AuthResponseDto { IsSuccess = true, Message = "Kayıt başarılı." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserNameOrEmail)
                       ?? await _userManager.FindByEmailAsync(dto.UserNameOrEmail);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized(new AuthResponseDto { IsSuccess = false, Message = "Kullanıcı adı veya şifre hatalı." });

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "CineSpectra_Super_Secret_Key_For_Jwt_Auth_2026"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.ToList()
            });
        }
    }
}