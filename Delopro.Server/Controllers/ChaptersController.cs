using AutoMapper;

using Delopro.Data.Interfaces;
using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Memory;
using Delopro.Server.Configuration;

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

            _memoryCache.Remove(CacheKeys.ChaptersKey);
            _memoryCache.Remove(CacheKeys.ChapterNodesKey);

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
                _memoryCache.Remove(CacheKeys.ChaptersKey);
                _memoryCache.Remove(CacheKeys.ChapterNodesKey);
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
            if (!_memoryCache.TryGetValue(CacheKeys.ChaptersKey, out IEnumerable<ChapterResponseModel>? response))
            {
                try
                {
                    var chapters = await _chapterRepository.GetListAsync();

                    if (chapters == null)
                    {
                        return StatusCode(500, new { errorText = "Ошибка сервера" });
                    }

                    response = _mapper.Map<IEnumerable<ChapterResponseModel>>(chapters);
                    _memoryCache.Set(CacheKeys.ChaptersKey, response, TimeSpan.FromMinutes(5));
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { errorText = ex.Message });
                }                
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetNodes()
        {
            if (!_memoryCache.TryGetValue(CacheKeys.ChapterNodesKey, out IEnumerable<ChapterNode>? response))
            {
                var chapters = await _chapterRepository.GetListAsync();

                if (chapters == null)
                {
                    return StatusCode(500, new { errorText = "Ошибка сервера" });
                }

                response = _mapper.Map<IEnumerable<ChapterNode>>(chapters);
                _memoryCache.Set(CacheKeys.ChapterNodesKey, response, TimeSpan.FromMinutes(5));
            }

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
                _memoryCache.Remove(CacheKeys.ChaptersKey);
                _memoryCache.Remove(CacheKeys.ChapterNodesKey);
            }
            catch (SqlException)
            {
                return StatusCode(500, new { errorText = "Ошибка базы данных" });
            }

            return Ok();
        }
    }
}
