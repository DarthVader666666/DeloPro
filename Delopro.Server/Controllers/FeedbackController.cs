using AutoMapper;
using Delopro.Bll.Interfaces;
using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Data.Interfaces;
using Delopro.Server.Attributes;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Send([FromForm] MessageForm? messageForm)
        {
            if (messageForm == null)
            {
                return BadRequest(new { errorText = "Не пришла форма сообщения" });
            }

            var email = _configuration["OwnerEmail"];

            if (email != null)
            {
                _emailSender.SendEmail(email, 
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

        [HttpGet]
        [Route("[action]/{isRead:bool}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetMessages([FromRoute] bool isRead)
        {
            var messages = (await _messageRepository.GetListAsync()).Where(message => message?.IsRead == isRead)
                .Select(message =>
                {
                    if (message == null)
                    {
                        return null;
                    }

                    message.Name = _cryptoService.Decrypt(message.Name);
                    message.Email = _cryptoService.Decrypt(message.Email);
                    message.Phone = _cryptoService.Decrypt(message.Phone);
                    message.Text = _cryptoService.Decrypt(message.Text);

                    return message;
                });

            try
            {
                var messageResponse = _mapper.Map<IEnumerable<MessageResponse>>(messages);
                return Ok(messageResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
            }
        }

        [HttpGet]
        [Route("[action]/{messageId:int}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetMessage([FromRoute] int messageId)
        {
            var message = await _messageRepository.GetAsync(messageId);

            if (message == null)
            {
                return NotFound(new { errorText = "Сообщение не найдено" });
            }

            message.Name = _cryptoService.Decrypt(message.Name);
            message.Email = _cryptoService.Decrypt(message.Email);
            message.Phone = _cryptoService.Decrypt(message.Phone);
            message.Text = _cryptoService.Decrypt(message.Text);

            try
            {
                var messageResponse = _mapper.Map<MessageResponse>(message);
                return Ok(messageResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
            }
        }

        [HttpPut]
        [Route("[action]/{messageId:int}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateMessage([FromRoute] int messageId)
        {
            var message = await _messageRepository.GetAsync(messageId);

            if (message == null)
            {
                return NotFound(new { errorText = "Сообщение не найдено" });
            }

            try
            {
                message.IsRead = true;
                var messageResult = await _messageRepository.UpdateAsync(message);

                if (messageResult != null)
                {
                    return Ok();
                }
                else 
                {
                    return StatusCode(500, new { errorText = "Ошибка сервера" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
            }
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetUnreadMessagesCount()
        {
            try
            {
                var count = (await _messageRepository.GetListAsync()).Count(message => !(message?.IsRead ?? true));

                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
            }
        }
    }
}
