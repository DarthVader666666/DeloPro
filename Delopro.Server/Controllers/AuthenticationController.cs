using Delopro.Bll.Interfaces;
using Delopro.Bll.Services;
using Delopro.Server.Attributes;
using Delopro.Server.Configuration;
using Delopro.Server.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Delopro.Server.Controllers
{
    [EnableCors("AllowClient")]    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IMemoryCache _memoryCache;

        public AuthenticationController(UserManager userManager, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> LogIn([FromBody] LogInRequest? logInRequest)
        {
            var user = await _userManager.GetUserByAsync(nickname: logInRequest?.Nickname, email: logInRequest?.Email);

            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }

            if (!user.IsConfirmed)
            {
                return NotFound(new { errorText = "Пользователь не подтвержден" });
            }

            if (!_userManager.IsMatchPassword(user, logInRequest?.Password))
            {
                return BadRequest(new { errorText = "Неверный пароль" });
            }

            if (!await _userManager.LogIn(user, HttpContext, logInRequest?.Remember ?? false))
            {
                return BadRequest(new { errorText = "Couldn't get user identity." });
            }

            return Ok(new { nickname = user.Nickname });
        }

        [HttpPost]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await UserManager.LogOut(HttpContext);
                _memoryCache.Remove(CacheKeys.CurrentUserKey);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = $"Logout failed. {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<bool> CheckAuthentication()
        {
            return UserManager.IsAuthenticated(HttpContext);
        }
    }
}