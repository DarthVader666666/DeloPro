using System.Text.Json;

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
using Microsoft.IdentityModel.Tokens;

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
        public async Task<IActionResult> GetCurrentUser()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.CurrentUserKey, out UserAccountResponseModel? userAccountResponseModel))
            {
                var user = await _userManager.GetCurrentUserAsync(HttpContext);

                if (user == null)
                {
                    _memoryCache.Remove(CacheKeys.CurrentUserKey);
                    return Ok();
                }

                userAccountResponseModel = _mapper.Map<UserAccountResponseModel>(user);
                _memoryCache.Set(CacheKeys.CurrentUserKey, userAccountResponseModel, TimeSpan.FromMinutes(5));
            }

            return Ok(userAccountResponseModel);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize]
        [TrackIpAddress]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserAccountUpdateModel? userAccountUpdateModel)
        {
            try
            {
                var user = await _userManager.GetCurrentUserAsync(HttpContext);

                if (user is null || userAccountUpdateModel is null)
                {
                    return StatusCode(500, new { errorText = "Ошибка сервера" });
                }

                user.Nickname = userAccountUpdateModel.Nickname;
                user.FirstName = _cryptoService.Encrypt(userAccountUpdateModel.FirstName);
                user.LastName = _cryptoService.Encrypt(userAccountUpdateModel.LastName);
                user.BirthDate = userAccountUpdateModel.BirthDate;
                user.Country = userAccountUpdateModel.Country;
                user.City = userAccountUpdateModel.City;
                user.UserTitle = userAccountUpdateModel.UserTitle;
                user.Info = userAccountUpdateModel.Info;
                user.Email = _cryptoService.Encrypt(userAccountUpdateModel.Email);
                user.Phone = _cryptoService.Encrypt(userAccountUpdateModel.Phone);

                await _userRepository.UpdateAsync(user);
                _memoryCache.Remove(CacheKeys.CurrentUserKey);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            return StatusCode(200, new { okText = $"Данные пользователя {user.Nickname} успешно обговлены"});
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize]
        [TrackIpAddress]
        public async Task<IActionResult> UpdateAvatar(IFormFile? avatar) 
        {
            var user = await _userManager.GetCurrentUserAsync(HttpContext);

            if (user is null)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            var oldAvatars = Directory.GetFiles(ConfigurationHelper.AvatarsPath!, $"user_{user.UserId}*");

            foreach (var oldAvatar in oldAvatars)
            {
                System.IO.File.Delete(oldAvatar);
            }

            if (avatar is not null)
            {
                var fileName = avatar?.FileName ?? String.Empty;
                var filePath = Path.Combine(ConfigurationHelper.AvatarsPath!, fileName);

                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                await avatar.CopyToAsync(stream);

                user.AvatarPath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                    ? $"/src/assets/avatars/{fileName}"
                    : filePath.Replace(ConfigurationHelper.WebRootPath!, string.Empty);
            }
            else 
            {
                user.AvatarPath = null;
            }

            await _userRepository.UpdateAsync(user);
            _memoryCache.Remove(CacheKeys.CurrentUserKey);

            return StatusCode(200, new { okText = $"Данные пользователя {user.Nickname} успешно обговлены" });
        }
    }
}
