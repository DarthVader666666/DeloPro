using AutoMapper;
using Delopro.Bll.Interfaces;
using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Data.Interfaces;
using Delopro.Server.Attributes;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly CryptoService _cryptoService;
        private readonly IRepository<Message> _messageRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public FeedbackController(IEmailSender emailSender, CryptoService cryptoService,
            IRepository<Message> messageRepository, IMapper mapper, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _cryptoService = cryptoService;
            _messageRepository = messageRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        [Route("[action]")]
        [HttpPost]
        [TrackIpAddress]
        public async Task<IActionResult> SendFeedback([FromForm] MessageForm? messageForm)
        {
            if (messageForm == null)
            {
                return BadRequest(new { errorText = "Не пришла форма сообщения" });
            }

            var email = _configuration["OwnerEmail"];

            if (email != null)
            {
                await _emailSender.SendEmailAsync(email, 
                    $"{messageForm.Name} прислал(а) сообщение в DeloPro",
                    $"<div>{messageForm.Text}</div>" +
                    (messageForm.Email.IsNullOrEmpty() ? "" : $"<div>Email: {messageForm.Email}</div>") +
                    (messageForm.Phone.IsNullOrEmpty() ? "" : $"<div>Телефон: {messageForm.Phone}</div>")
                );
            }

            Message? createdMessage;

            try
            {
                var message = _mapper.Map<Message>(messageForm);

                message.Name = _cryptoService.Encrypt(message.Name);
                message.Email = _cryptoService.Encrypt(message.Email);
                message.Phone = _cryptoService.Encrypt(message.Phone);
                message.Text = _cryptoService.Encrypt(message.Text);

                createdMessage = await _messageRepository.CreateAsync(message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
            }

            if (createdMessage != null)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка отправки сообщения" });
            }
        }
    }
}
