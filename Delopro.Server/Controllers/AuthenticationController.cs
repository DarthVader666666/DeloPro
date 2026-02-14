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
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _memoryCache;

        public AuthenticationController(UserManager userManager, IEmailSender emailSender, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> LogIn([FromBody] UserLogInRequest? userLogInRequest)
        {
            var user = await _userManager.GetUserByAsync(nickname: userLogInRequest?.Nickname, email: userLogInRequest?.Email);

            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }

            if (!user.IsConfirmed)
            {
                return NotFound(new { errorText = "Пользователь не подтвержден" });
            }

            if (!_userManager.IsMatchPassword(user, userLogInRequest?.Password))
            {
                return BadRequest(new { errorText = "Неверный пароль" });
            }

            if (!await _userManager.LogIn(user, HttpContext, userLogInRequest?.Remember ?? false))
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

        [HttpPost]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> RecoverPassword()
        {
            var headers = HttpContext.Request.Headers;

            if (headers is null)
            { 
                return BadRequest();
            }

            var email = headers["Email"].ToString();
            var userExists = await _userManager.DoesUserExistAsync(email, doEncrypt: true);

            if (!userExists)
            {
                return NotFound(new { errorText = $"Пользователь с email \"{email}\" не найден" });
            }

            var password = UserManager.GeneratePassword();

            if (!_emailSender.SendEmail(email, "Восстановление пароля", $"Ваш новый пароль:\n\r{password}"))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка отправки сообщения" });
            }

            try
            {
                var user = await _userManager.GetUserByAsync(email: email);
                await _userManager.ChangePasswordAsync(user, password);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка при изменении пароля" });
            }            

            return Ok("Сообщение успешно отправлено");
        }
    }
}
