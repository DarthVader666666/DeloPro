using Delopro.Data.Interfaces;
using Delopro.Data.Entities;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly IRepository<Captcha> _captchaRepository;

        public CaptchaController(IRepository<Captcha> captchaRepository)
        {
            _captchaRepository = captchaRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Get()
        {
            var captcha = await _captchaRepository.GetAsync(null);

            if (captcha != null)
            {
                return Ok(captcha);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
            }
        }
    }
}
