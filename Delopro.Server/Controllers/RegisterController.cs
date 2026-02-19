using System.Text;

using AutoMapper;

using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Server.Attributes;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    public class RegisterController: ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public RegisterController(UserManager userManager, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest registerRequest)
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

                var result = await _userManager.RegisterAsync(user, HttpContext?.Request?.GetDisplayUrl().Replace("RegisterUser", string.Empty, StringComparison.OrdinalIgnoreCase));

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
        public async Task<IActionResult> ConfirmUser([FromQuery] int[]? key1, [FromQuery] int[]? key2)
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
        public async Task<IActionResult> CheckUserExists([FromQuery] string? nickname, [FromQuery] string? email)
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
