using AuthService.Models;
using AuthService.Models.Requests;
using AuthService.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;


namespace AuthService.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtService _jwtService;
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public AuthController(UserManager<User> userManager, JwtService jwtService, IDistributedCache cache)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _cache = cache;
            _cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!UserRoles.AllRoles.Contains(request.Role))
                return BadRequest("Недопустимая роль");

            if (request.Role == UserRoles.Admin && !await ValidateAdminCreation())
                return Forbid("Только администраторы могут создавать других администраторов");

            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                Role = request.Role
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            var tokens = await GenerateTokens(user);
            var accessToken = tokens.GetType().GetProperty("AccessToken")?.GetValue(tokens)?.ToString();

            if(accessToken == null)
            {
                await _userManager.DeleteAsync(user);
                return StatusCode(500, "Ошибка при генерации токенов");
            }


            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var profileData = new
                {
                    UserId = user.Id,
                    FirstName="Default",
                    LastName="Default",
                    Phone="123456790",
                };

                var response = await httpClient.PostAsJsonAsync(
                        "http://profile-service/api/profiles",
                        profileData);

                if (!response.IsSuccessStatusCode)
                {
                    await _userManager.DeleteAsync(user);
                    return StatusCode(500, "Ошибка при создании профиля");
                }

            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                return StatusCode(500, "Ошибка связи с profile-service");
            }


            await CacheUserData(user);

            return Ok(new { Message = "Пользователь успешно зарегистрирован" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {

            var cachedUser = await GetCachedUser(request.Email);
            if (cachedUser != null)
            {
                if (await ValidateCachedUser(cachedUser, request.Password))
                {
                    return await GenerateTokensResponse(cachedUser);
                }
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized();
            }

            await CacheUserData(user);

            return await GenerateTokensResponse(user);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequest request)
        {
            try
            {
                var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

                var cachedUser = await GetCachedUserById(userId);
                if (cachedUser != null && ValidateCachedRefreshToken(cachedUser, request.RefreshToken))
                    return await RefreshTokens(cachedUser);

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.RefreshToken != request.RefreshToken ||
                    user.RefreshTokenExpiry <= DateTime.UtcNow)
                    return Unauthorized();

                await CacheUserData(user);

                return await RefreshTokens(user);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { Error = ex.Message });
            }
        }


        private async Task<bool> ValidateAdminCreation()
        {
            if (!User.Identity.IsAuthenticated) return false;

            var currentUser = await _userManager.GetUserAsync(User);
            return currentUser != null &&
                   await _userManager.IsInRoleAsync(currentUser, UserRoles.Admin);
        }

        private async Task CacheUserData(User user)
        {
            var cachedData = new
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                RefreshTokens = user.RefreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiry
            };

            await _cache.SetStringAsync(
                $"user_{user.Id}",
                JsonSerializer.Serialize(cachedData),
                _cacheOptions
                );
        }

        private async Task<User> GetCachedUser(string email)
        {
            try
            {
                var cachedData = await _cache.GetStringAsync($"user_email_{email}");
                return cachedData == null ? null : JsonSerializer.Deserialize<User>(cachedData);
            }
            catch
            {
                return null;
            }
        }

        private async Task<User> GetCachedUserById(string userId)
        {
            try
            {
                var cachedData = await _cache.GetStringAsync($"user_{userId}");
                return cachedData == null ? null : JsonSerializer.Deserialize<User>(cachedData);
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> ValidateCachedUser(User user, string password)
        {
            return user.PasswordHash == _userManager.PasswordHasher.HashPassword(user, password);
        }

        private bool ValidateCachedRefreshToken(User user, string refreshToken)
        {
            return user.RefreshToken == refreshToken &&
                   user.RefreshTokenExpiry > DateTime.UtcNow;
        }

        private async Task<IActionResult> GenerateTokensResponse(User user)
        {
            var tokens = await GenerateTokens(user);
            return Ok(tokens);
        }

        private async Task<object> GenerateTokens(User user)
        {
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtService.GetRefreshExpireDays());

            await _userManager.UpdateAsync(user);
            await CacheUserData(user);

            return new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<IActionResult> RefreshTokens(User user)
        {
            var tokens = await GenerateTokens(user);
            return Ok(tokens);
        }

    }
}
