using AutoMapper;

using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Data.Interfaces;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [Authorize(Roles = "Owner")]
    public class MessagesController: ControllerBase
    {
        private readonly CryptoService _cryptoService;
        private readonly IRepository<Message> _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(CryptoService cryptoService, IRepository<Message> messageRepository, IMapper mapper)
        {
            _cryptoService = cryptoService;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("[action]/{isRead:bool}")]
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
        public async Task<IActionResult> UpdateMessage([FromRoute] int messageId)
        {
            var message = await _messageRepository.GetAsync(messageId);

            if (message == null)
            {
                return NotFound(new { errorText = "Сообщение не найдено" });
            }

            message.IsRead = true;
            var messageResult = await _messageRepository.UpdateAsync(message);

            if (messageResult != null)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
            }
        }

        [HttpGet]
        [Route("[action]")]
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
