using AutoMapper;
using Delopro.Data.Interfaces;
using Delopro.Bll.Services;
using Delopro.Data.Entities;
using Delopro.Server.Attributes;
using Delopro.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Delopro.Server.Configuration;

namespace Delopro.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("api/[controller]")]
    [ApiController]
    public class ThemesController : ControllerBase
    {
        private const string BaseThemeKey = "theme_id=";

        private readonly IRepository<Theme> _themeRepository;
        private readonly UserManager _userManager;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;        

        public ThemesController(IRepository<Theme> themesRepository, UserManager userManager, IMapper mapper, IMemoryCache memoryCache)
        {
            _themeRepository = themesRepository;
            _userManager = userManager;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("[action]/{themeId:int}")]
        [TrackIpAddress]
        public async Task<IActionResult> GetTheme(int themeId)
        {
            if (!_memoryCache.TryGetValue($"{BaseThemeKey}{themeId}", out Theme? theme))
            {
                theme = await _themeRepository.GetAsync(themeId);

                _memoryCache.Set($"{BaseThemeKey}{themeId}", theme, TimeSpan.FromMinutes(5));
            }

            return Ok(theme);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetThemes([FromQuery] int? chapterId = null)
        {
            var themes = await _themeRepository.GetListAsync(chapterId);

            return Ok(themes);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> CreateTheme(ThemeCreateRequest themeCreateRequest)
        {
            try
            {
                var theme = _mapper.Map<Theme>(themeCreateRequest);
                var userId = (await _userManager.GetCurrentUserAsync(HttpContext))?.UserId;
                theme.UserId = userId;

                await _themeRepository.CreateAsync(theme);

                _memoryCache.Remove(CacheKeys.ChaptersKey);
                _memoryCache.Remove(CacheKeys.ChapterNodesKey);
            }
            catch (SqlException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка Базы данных" });
            }

            return Ok();
        }

        [HttpDelete]
        [Route("[action]/{themeId:int}")]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> DeleteTheme([FromRoute] int? themeId) 
        {
            if (themeId == null)
            {
                return BadRequest(new { errorText = "Запрос неверен" });
            }

            try
            {
                await _themeRepository.DeleteAsync(themeId);

                _memoryCache.Remove(CacheKeys.ChaptersKey);
                _memoryCache.Remove(CacheKeys.ChapterNodesKey);
            }
            catch (SqlException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка базы данных" });
            }

            return Ok();
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> UpdateTheme([FromBody] ThemeUpdateRequest themeUpdateRequest)
        {
            var theme = _mapper.Map<Theme>(themeUpdateRequest);

            try
            {
                await _themeRepository.UpdateAsync(theme);
            }
            catch (SqlException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка базы данных" });
            }

            _memoryCache.Remove($"{BaseThemeKey}{theme.ThemeId}");
            _memoryCache.Remove(CacheKeys.ChaptersKey);
            _memoryCache.Remove(CacheKeys.ChapterNodesKey);

            return Ok();
        }
    }
}
