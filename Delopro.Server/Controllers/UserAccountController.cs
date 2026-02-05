using System.Text.Json;

using AutoMapper;

using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Data.Interfaces;
using Delopro.Server.Attributes;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Delopro.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("api/[controller]")]
    public class UserAccountController: ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UserAccountController(UserManager userManager, IRepository<User> userRepository, IMapper mapper)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetCurrentUserAsync(HttpContext);

            if (user == null)
            {
                return StatusCode(400, new { errorText = "Пользователь не найден" });
            }

            var userLongResponseModel = _mapper.Map<UserAccountResponseModel>(user);

            return Ok(userLongResponseModel);
        }

        [HttpPut]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> UpdateCurrentUser([FromForm] UserAccountUpdateRequestModel userAccountUpdateRequestModel)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var user = JsonSerializer.Deserialize<UserAccountUpdateModel>(userAccountUpdateRequestModel.User!, options);

            if (user == null)
            {
                return StatusCode(400, new { errorText = "Ошибка сервера" });
            }

            return Ok();
        }
    }
}
