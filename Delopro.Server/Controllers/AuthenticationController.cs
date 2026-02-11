using Delopro.Bll.Interfaces;
using Delopro.Bll.Services;
using Delopro.Server.Attributes;
using Delopro.Server.Configuration;
using Delopro.Server.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json;

using System.Security.Claims;
using System.Text;

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
        public async Task<IActionResult> LogIn([FromQuery]string? nickname = null, [FromQuery] bool remember = false)
        {
            var userLogInRequestModel = JsonConvert.DeserializeObject<UserLogInRequestModel>(HttpContext.Request.Headers["Authentication"].ToString());
            var password = Encoding.UTF8.GetString(userLogInRequestModel?.Password ?? []);

            var user = await _userManager.GetUserByAsync(nickname: nickname, email: userLogInRequestModel?.Email);

            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }

            if (!user.IsConfirmed)
            {
                return NotFound(new { errorText = "Пользователь не подтвержден" });
            }

            if (!_userManager.IsMatchPassword(user, password))
            {
                return BadRequest(new { errorText = "Неверный пароль" });
            }

            if (!await _userManager.LogIn(user, HttpContext, remember))
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
                await _userManager.LogOut(HttpContext);
                _memoryCache.Remove(CacheKeys.CurrentUserKey);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorText = $"Logout failed. {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> RecoverPassword()
        {
            var email = HttpContext.Request.Headers["Email"].ToString();
            var userExists = await _userManager.DoesUserExistAsync(email, doEncrypt: true);

            if (!userExists)
            {
                return BadRequest(new { errorText = $"Пользователь с email \"{email}\" не найден" });
            }

            var password = _userManager.GeneratePassword();

            if (!_emailSender.SendEmail(email, "Восстановление пароля", $"Ваш новый пароль:\n\r{password}"))
            {
                return StatusCode(500, new { errorText = "Ошибка отправки сообщения" });
            }

            try
            {
                var user = await _userManager.GetUserByAsync(email: email);
                await _userManager.ChangePasswordAsync(user, password);
            }
            catch
            {
                return StatusCode(500, new { errorText = "Ошибка при изменении пароля" });
            }            

            return Ok("Сообщение успешно отправлено");
        }
    }
}
