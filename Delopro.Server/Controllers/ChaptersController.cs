using AutoMapper;

using Delopro.Data.Interfaces;
using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Delopro.Server.Configuration;
using Delopro.Server.Attributes;

namespace Delopro.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public ChaptersController(UserManager userManager, IRepository<Chapter> chapterRepository, IMapper mapper, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _chapterRepository = chapterRepository;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        [Authorize(Roles = "Owner, Admin")]
        [Route("[action]")]
        public async Task<IActionResult> CreateChapter([FromForm] ChapterCreateRequest chapterCreateRequest)
        {
            if (chapterCreateRequest == null || string.IsNullOrEmpty(chapterCreateRequest.ChapterTitle) || chapterCreateRequest.DateCreated == null)
            {
                return BadRequest(new { errorText = "Неверные данные для создания раздела" } );
            }

            var user = await _userManager.GetCurrentUserAsync(HttpContext);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
            }

            var chapter = new Chapter
            {
                ChapterTitle = chapterCreateRequest.ChapterTitle,
                ImagePath = chapterCreateRequest.ImagePath,
                DateCreated = chapterCreateRequest.DateCreated ?? DateTime.Now,
                UserId = user.UserId
            };

            var createdChapter = await _chapterRepository.CreateAsync(chapter);

            if (createdChapter == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
            }

            _memoryCache.Remove(CacheKeys.ChaptersKey);
            _memoryCache.Remove(CacheKeys.ChapterNodesKey);

            return Ok(createdChapter);
        }


        [HttpDelete]
        [Route("[action]/{chapterId:int}")]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> DeleteChapter(int chapterId)
        {
            try
            {
                await _chapterRepository.DeleteAsync(chapterId);
                _memoryCache.Remove(CacheKeys.ChaptersKey);
                _memoryCache.Remove(CacheKeys.ChapterNodesKey);
            }
            catch (SqlException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка базы данных" });
            }

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> GetChapters() 
        {
            if (!_memoryCache.TryGetValue(CacheKeys.ChaptersKey, out IEnumerable<ChapterResponse>? chapterResponse))
            {
                var chapters = await _chapterRepository.GetListAsync();

                if (chapters == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
                }

                chapterResponse = _mapper.Map<IEnumerable<ChapterResponse>>(chapters);
                _memoryCache.Set(CacheKeys.ChaptersKey, chapterResponse, TimeSpan.FromMinutes(5));
            }

            return Ok(chapterResponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetChapterNodes()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.ChapterNodesKey, out IEnumerable<ChapterNode>? chapterNodes))
            {
                var chapters = await _chapterRepository.GetListAsync();

                if (chapters == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
                }

                chapterNodes = _mapper.Map<IEnumerable<ChapterNode>>(chapters);
                _memoryCache.Set(CacheKeys.ChapterNodesKey, chapterNodes, TimeSpan.FromMinutes(5));
            }

            return Ok(chapterNodes);
        }

        [HttpGet]
        [Route("[action]/{chapterId:int}")]
        public async Task<IActionResult> GetChapter(int chapterId)
        {
            var chapter = await _chapterRepository.GetAsync(chapterId);

            if (chapter == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка сервера" });
            }

            var chapterResponseModel = _mapper.Map<ChapterResponse>(chapter);

            return Ok(chapterResponseModel);
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> UpdateChapter([FromBody] ChapterUpdateRequest chapterUpdateRequest)
        {   
            var chapter = _mapper.Map<Chapter>(chapterUpdateRequest);

            try
            {
                await _chapterRepository.UpdateAsync(chapter);
                _memoryCache.Remove(CacheKeys.ChaptersKey);
                _memoryCache.Remove(CacheKeys.ChapterNodesKey);
            }
            catch (SqlException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка базы данных" });
            }

            return Ok();
        }
    }
}
