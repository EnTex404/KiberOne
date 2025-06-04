using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Models;
using System.Security.Claims;

namespace ProfileService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/profiles")]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileDbContext _context;

        public ProfileController(ProfileDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId не найден в токене");

            var profile = await _context.Profile.FirstOrDefaultAsync(x => x.UserId == userId);
            if (profile == null)
                return NotFound("Профиль не найден");

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] Profile profileData)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId не найден в токене");

            if (await _context.Profile.AnyAsync(x => x.UserId == userId))
                return Conflict("Профиль уже существует");

            var profile = new Profile
            {
                UserId = userId,
                FirstName = profileData.FirstName,
                LastName = profileData.LastName,
                Phone = profileData.Phone,
                CreatedAt = DateTime.UtcNow
            };

            _context.Profile.Add(profile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Профиль создан" });
        }
    }
}
