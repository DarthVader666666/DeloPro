using AutoMapper;

using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Data.Interfaces;
using Delopro.Server.Attributes;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    public class CommentsController: ControllerBase
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly UserManager _userManager;
        private readonly IMapper _autoMapper;

        public CommentsController(IRepository<Comment> commentRepository, UserManager userManager, IMapper autoMapper)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
            _autoMapper = autoMapper;
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner,Admin,User")]
        [TrackIpAddress]
        public async Task<IActionResult> CreateComment([FromBody] CommentCreateRequest commentCreateRequest) 
        {
            try
            {
                var comment = new Comment();
                var user = await _userManager.GetCurrentUserAsync(HttpContext);

                comment.ThemeId = commentCreateRequest.ThemeId;
                comment.Text = commentCreateRequest.Text;
                comment.UserId = user!.UserId;
                comment.DateCreated = DateTime.Now;

                await _commentRepository.CreateAsync(comment);
            }
            catch
            {
                return StatusCode(500, new { errorText = "Ошибка создания комментария" });
            }

            return Ok(new { okText = "Ваш комментарий опубликован" });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetComments([FromQuery] int themeId)
        {
            try
            {
                var comments = await _commentRepository.GetListIncludeAsync(themeId);
                var response = _autoMapper.Map<IEnumerable<CommentResponse>>(comments);
                return Ok(comments);
            }
            catch 
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }
        }
    }
}
