using AutoMapper;

using Delopro.Bll;
using Delopro.Bll.Interfaces;
using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Data.Interfaces;
using Delopro.Server.Attributes;
using Delopro.Server.Configuration;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Delopro.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("api/[controller]")]
    public class AccountController: ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IRepository<User> _userRepository;
        private readonly CryptoService _cryptoService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager userManager, IRepository<User> userRepository, CryptoService cryptoService, IMapper mapper, IMemoryCache memoryCache, IEmailSender emailSender)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _cryptoService = cryptoService;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _emailSender = emailSender;
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        [TrackIpAddress]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetCurrentUserAsync(HttpContext);

            if (user == null)
            {
                _memoryCache.Remove(CacheKeys.CurrentUserKey);
                return Ok();
            }

            var userAccountResponse = _mapper.Map<AccountResponse>(user);

            return Ok(userAccountResponse);
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize]
        //[TrackIpAddress]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] AccountUpdateRequest? accountUpdateRequest)
        {
            var user = await _userManager.GetCurrentUserAsync(HttpContext);

            try
            {
                if (user is null || accountUpdateRequest is null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
                }

                user.Nickname = accountUpdateRequest.Nickname;
                user.FirstName = _cryptoService.Encrypt(accountUpdateRequest.FirstName);
                user.LastName = _cryptoService.Encrypt(accountUpdateRequest.LastName);
                user.BirthDate = accountUpdateRequest.BirthDate;
                user.Country = accountUpdateRequest.Country;
                user.City = accountUpdateRequest.City;
                user.UserTitle = accountUpdateRequest.UserTitle;
                user.Info = accountUpdateRequest.Info;
                user.Email = _cryptoService.Encrypt(accountUpdateRequest.Email);
                user.Phone = _cryptoService.Encrypt(accountUpdateRequest.Phone);

                await _userRepository.UpdateAsync(user);
                _memoryCache.Remove(CacheKeys.CurrentUserKey);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
            }

            return Ok(new { okText = $"Данные пользователя {user.Nickname} успешно обновлены"});
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize]
        //[TrackIpAddress]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile? avatar) 
        {
            if (avatar is null)
            {
                return BadRequest();
            }

            var user = await _userManager.GetCurrentUserAsync(HttpContext);

            if (user is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
            }

            DeleteOldAvatars(user.UserId);

            var fileName = avatar?.FileName ?? string.Empty;
            var filePath = Path.Combine(ConfigurationHelper.AvatarsPath!, fileName);

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            await avatar!.CopyToAsync(stream);

            user.AvatarPath = fileName;                

            await _userRepository.UpdateAsync(user);
            _memoryCache.Remove(CacheKeys.CurrentUserKey);

            return Ok(new { okText = $"Данные пользователя {user.Nickname} успешно обговлены" });
        }

        [HttpDelete]
        [Route("[action]")]
        [Authorize]
        //[TrackIpAddress]
        public async Task<IActionResult> DeleteAvatar()
        {
            var user = await _userManager.GetCurrentUserAsync(HttpContext);

            if (user is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
            }

            DeleteOldAvatars(user.UserId);

            user.AvatarPath = null;

            await _userRepository.UpdateAsync(user);
            _memoryCache.Remove(CacheKeys.CurrentUserKey);

            return Ok(new { okText = $"Данные пользователя {user.Nickname} успешно обговлены" });
        }

        [HttpPost]
        [Route("[action]")]
        //[TrackIpAddress]
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

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public async Task<IActionResult> CheckPassword()
        { 
            var password = HttpContext.Request.Headers["Password"].ToString();

            var currentUser = await _userManager.GetCurrentUserAsync(HttpContext);
            var result = currentUser is not null && _cryptoService.Encrypt(password) == currentUser.Password;

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [Route("[action]")]
        public async Task<IActionResult> ChangePassword()
        {
            var headers = HttpContext?.Request?.Headers;

            if (headers is null)
            {
                return BadRequest("Новый пароль не указан");
            }

            var password = headers["Password"].ToString();

            try
            {
                var currentUser = await _userManager.GetCurrentUserAsync(HttpContext);
                currentUser!.Password = _cryptoService.Encrypt(password);
                await _userRepository.UpdateAsync(currentUser);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка при смене пароля" });
            }

            return Ok(new { okText = "Пароль успешно обновлён" });
        }

        [HttpDelete]
        [Authorize]
        [Route("[action]")]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var currentUser = await _userManager.GetCurrentUserAsync(HttpContext);
                await _userRepository.DeleteAsync(currentUser!.UserId);

                return Ok(new { okText = "Аккаунт успешно удалён" });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка при удалении аккаунта" });
            }
        }

        private static void DeleteOldAvatars(int userId) 
        {
            var oldAvatars = Directory.GetFiles(ConfigurationHelper.AvatarsPath!, $"user_{userId}*");

            foreach (var oldAvatar in oldAvatars)
            {
                System.IO.File.Delete(oldAvatar);
            }
        }
    }
}
