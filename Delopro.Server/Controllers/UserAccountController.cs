using AutoMapper;

using Delopro.Bll;
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
    public class UserAccountController: ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IRepository<User> _userRepository;
        private readonly CryptoService _cryptoService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public UserAccountController(UserManager userManager, IRepository<User> userRepository, CryptoService cryptoService, IMapper mapper, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _cryptoService = cryptoService;
            _mapper = mapper;
            _memoryCache = memoryCache;
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

            var userAccountResponse = _mapper.Map<UserAccountResponse>(user);

            return Ok(userAccountResponse);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize]
        [TrackIpAddress]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserAccountUpdateRequest? userAccountUpdateRequest)
        {
            var user = await _userManager.GetCurrentUserAsync(HttpContext);

            try
            {
                if (user is null || userAccountUpdateRequest is null)
                {
                    return StatusCode(500, new { errorText = "Ошибка сервера" });
                }

                user.Nickname = userAccountUpdateRequest.Nickname;
                user.FirstName = _cryptoService.Encrypt(userAccountUpdateRequest.FirstName);
                user.LastName = _cryptoService.Encrypt(userAccountUpdateRequest.LastName);
                user.BirthDate = userAccountUpdateRequest.BirthDate;
                user.Country = userAccountUpdateRequest.Country;
                user.City = userAccountUpdateRequest.City;
                user.UserTitle = userAccountUpdateRequest.UserTitle;
                user.Info = userAccountUpdateRequest.Info;
                user.Email = _cryptoService.Encrypt(userAccountUpdateRequest.Email);
                user.Phone = _cryptoService.Encrypt(userAccountUpdateRequest.Phone);

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
        [TrackIpAddress]
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
        [TrackIpAddress]
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
