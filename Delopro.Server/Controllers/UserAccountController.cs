using System.Text.Json;

using AutoMapper;

using Delopro.Bll;
using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Data.Interfaces;
using Delopro.Server.Attributes;
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
        private const string CurrentUserKey = "currentUser";

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
            if (!_memoryCache.TryGetValue(CurrentUserKey, out UserAccountResponseModel? userAccountResponseModel))
            {
                var user = await _userManager.GetCurrentUserAsync(HttpContext);

                //if (user == null)
                //{
                //    return Ok("Пользователь не аутентифицирован");
                //}

                userAccountResponseModel = _mapper.Map<UserAccountResponseModel>(user);
                _memoryCache.Set(CurrentUserKey, userAccountResponseModel, TimeSpan.FromMinutes(5));
            }

            return Ok(userAccountResponseModel);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize]
        [TrackIpAddress]
        public async Task<IActionResult> UpdateCurrentUser([FromForm] UserAccountUpdateRequestModel userAccountUpdateRequestModel)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            UserAccountUpdateModel? userAccount;
            User? user;

            try
            {
                user = await _userManager.GetCurrentUserAsync(HttpContext);
                userAccount = JsonSerializer.Deserialize<UserAccountUpdateModel?>(userAccountUpdateRequestModel.User!, options);

                if (user is null || userAccount is null)
                {
                    return StatusCode(500, new { errorText = "Ошибка сервера" });
                }

                var fileName = userAccountUpdateRequestModel!.Avatar?.FileName ?? String.Empty;
                var filePath = Path.Combine(ConfigurationHelper.AvatarsPath!, fileName);

                user.Nickname = userAccount.Nickname;
                user.FirstName = _cryptoService.Encrypt(userAccount.FirstName);
                user.LastName = _cryptoService.Encrypt(userAccount.LastName);
                user.BirthDate = userAccount.BirthDate;
                user.Country = userAccount.Country;
                user.City = userAccount.City;
                user.UserTitle = userAccount.UserTitle;
                user.Info = userAccount.Info;
                user.Email = _cryptoService.Encrypt(userAccount.Email);
                user.Phone = _cryptoService.Encrypt(userAccount.Phone);

                if (userAccountUpdateRequestModel.Avatar is not null)
                {
                    if (userAccount.DeleteAvatar)
                    {
                        var oldAvatars = Directory.GetFiles(ConfigurationHelper.AvatarsPath!, $"user_{user.UserId}*");

                        foreach (var oldAvatar in oldAvatars)
                        {
                            System.IO.File.Delete(oldAvatar);
                        }
                    }
                    else
                    {
                        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                        await userAccountUpdateRequestModel.Avatar.CopyToAsync(stream);

                        user.AvatarPath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                            ? $"/src/assets/avatars/{fileName}"
                            : filePath.Replace(ConfigurationHelper.WebRootPath!, string.Empty);
                    }
                }                
                
                if(userAccount.DeleteAvatar)
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);                        
                    }

                    user.AvatarPath = null;
                }

                await _userRepository.UpdateAsync(user);
                _memoryCache.Remove(CurrentUserKey);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            return StatusCode(200, new { okText = $"Данные пользователя {user.Nickname} успешно обговлены"});
        }
    }
}
