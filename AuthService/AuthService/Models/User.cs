using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
    public class User : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public string Role { get; set; }
    }
}
