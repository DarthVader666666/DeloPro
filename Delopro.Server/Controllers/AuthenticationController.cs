using System.Text;

using AutoMapper;

using Delopro.Bll.Interfaces;
using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Server.Attributes;
using Delopro.Server.Configuration;
using Delopro.Server.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
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
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthenticationController(UserManager userManager, IEmailSender emailSender, IMemoryCache memoryCache, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _mapper = mapper;
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

        [HttpPost]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest is null)
            {
                return BadRequest(new { errorText = "Неверные данные в запросе" });
            }

            if (registerRequest?.Password == null)
            {
                return BadRequest(new { errorText = "Ошибка регистрации: Не указан пароль" });
            }

            if (registerRequest.Email == null)
            {
                return BadRequest(new { errorText = "Ошибка регистрации: Не указан Email" });
            }

            if (await _userManager.DoesUserExistAsync(registerRequest.Email))
            {
                return BadRequest(new { errorText = "Такой пользователь уже зарегестрирован" });
            }

            try
            {
                var user = _mapper.Map<User>(registerRequest);

                var result = await _userManager.RegisterAsync(user, HttpContext?.Request?.GetDisplayUrl());

                if (result)
                {
                    return Ok(new { okText = "Письмо отправлено" });
                }
                else
                {
                    return BadRequest(new { errorText = "Ошибка регистрации" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
            }
        }

        [HttpGet]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> RegisterConfirm([FromQuery] int[]? key1, [FromQuery] int[]? key2)
        {
            var confirmedUser = await _userManager.ConfirmUserAsync(
                [
                    Encoding.UTF8.GetString([.. (key1 ?? []).Select(x => (byte)x)]),
                    Encoding.UTF8.GetString([.. (key2 ?? []).Select(x => (byte)x)])
                ]
            );

            if (confirmedUser == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка подтверждения" });
            }

            await _userManager.LogIn(confirmedUser, HttpContext);

            return Redirect($"{_configuration["ClientUrl"]}");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> UserExists([FromQuery] string? nickname, [FromQuery] string? email)
        {
            bool userExists;

            if (nickname == null)
            {
                userExists = await _userManager.DoesUserExistAsync(email, doEncrypt: true);
            }
            else
            {
                userExists = await _userManager.DoesUserExistAsync(nickname);
            }

            if (userExists)
            {
                return Ok(new { userExists = true });
            }
            else
            {
                return Ok(new { userExists = false });
            }
        }
    }
}