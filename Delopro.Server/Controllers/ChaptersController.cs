using AutoMapper;

using Delopro.Bll.Interfaces;
using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace Delopro.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IRepository<Theme> _themeRepository;
        private readonly IMapper _mapper;

        public ChaptersController(UserManager userManager, IRepository<Chapter> chapterRepository, IRepository<Theme> themeRepository, IMapper mapper)
        {
            _userManager = userManager;
            _chapterRepository = chapterRepository;
            _themeRepository = themeRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Owner, Admin")]
        [Route("[action]")]
        public async Task<IActionResult> Create([FromForm] ChapterCreateModel chapterCreateModel)
        {
            if (chapterCreateModel == null || chapterCreateModel.ChapterTitle.IsNullOrEmpty() || chapterCreateModel.DateCreated == null)
            {
                return BadRequest(new { errorText = "Неверные данные для создания раздела" } );
            }

            var user = await _userManager.GetCurrentUserAsync(HttpContext);

            if (user == null)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            var chapter = new Chapter
            {
                ChapterTitle = chapterCreateModel.ChapterTitle,
                ImagePath = chapterCreateModel.ImagePath,
                DateCreated = chapterCreateModel.DateCreated ?? DateTime.Now,
                UserId = user.UserId
            };

            var createdChapter = await _chapterRepository.CreateAsync(chapter);

            if (createdChapter == null)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            return Ok(createdChapter);
        }


        [HttpDelete]
        [Route("[action]/{chapterId:int}")]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> Delete(int chapterId)
        {
            try
            {
                await _chapterRepository.DeleteAsync(chapterId);
            }
            catch (SqlException)
            {
                return StatusCode(500, new { errorText = "Ошибка базы данных" });
            }

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetList() 
        {
            IEnumerable<Chapter?> chapters;

            try
            {
                chapters = await _chapterRepository.GetListAsync();

                if (chapters == null)
                {
                    return StatusCode(500, new { errorText = "Ошибка сервера" });
                }
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { errorText = ex.Message });
            }

            var response = _mapper.Map<IEnumerable<ChapterResponseModel>>(chapters);

            return Ok(response);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetNodes()
        {
            var chapters = await _chapterRepository.GetListAsync();

            if (chapters == null)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            var response = _mapper.Map<IEnumerable<ChapterNode>>(chapters);

            return Ok(response);
        }

        [HttpGet]
        [Route("[action]/{chapterId:int}")]
        public async Task<IActionResult> Get(int chapterId)
        {
            var chapter = await _chapterRepository.GetAsync(chapterId);

            if (chapter == null)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            var chapterResponseModel = _mapper.Map<ChapterResponseModel>(chapter);

            return Ok(chapterResponseModel);
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> Update([FromBody] ChapterUpdateModel chapterUpdateModel)
        {   
            var chapter = _mapper.Map<Chapter>(chapterUpdateModel);

            try
            {
                await _chapterRepository.UpdateAsync(chapter);
            }
            catch (SqlException)
            {
                return StatusCode(500, new { errorText = "Ошибка базы данных" });
            }

            return Ok();
        }
    }
}
