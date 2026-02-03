using AutoMapper;

using Delopro.Bll.Services;
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
        private readonly IMapper _mapper;

        public UserAccountController(UserManager userManager, IMapper mapper)
        {
            _userManager = userManager;
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
    }
}
