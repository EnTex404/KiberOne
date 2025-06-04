using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Requests
{
    public class RefreshRequest
    {
        [Required]
        public string AccessToken { get; set; } = null!;

        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
