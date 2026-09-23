using Microsoft.AspNetCore.Identity;

namespace CineSpectra.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}