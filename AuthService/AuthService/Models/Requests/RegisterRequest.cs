using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Requests
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}
