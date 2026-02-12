using System.Net;
using System.Text.RegularExpressions;

using Delopro.Data.Interfaces;
using Delopro.Data.Entities;
using Delopro.Server.Attributes;
using Delopro.Server.Models;

using HtmlAgilityPack;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace Delopro.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IRepository<Theme> _themeRepository;

        public SearchController(IRepository<Theme> themeRepository)
        {
            _themeRepository = themeRepository;
        }

        [HttpPost]
        [Route("[action]")]
        [TrackIpAddress]
        public async Task<IActionResult> GetSearchResult()
        {
            string? searchLine = null;

            try
            {
                var reader = new StreamReader(HttpContext.Request.Body);
                searchLine = JsonSerializer.Deserialize<SearchLineModel>(await reader.ReadToEndAsync())?.SearchLine;

                if (searchLine == null || searchLine.Length < 3)
                {
                    return Ok(Enumerable.Empty<SearchResultModel>());
                }
            }
            catch
            {
                return StatusCode(500, new { errorText = "Не удалось прочесть запрос" });
            }

            if (searchLine == null)
            {
                return BadRequest(new { errorText = "Не задана строка поиска" });
            }

            string getPlainText(string html)
            {
                var tagsStripped = Regex.Replace(html, "<.*?>", string.Empty);
                var plainText = WebUtility.HtmlDecode(tagsStripped);

                return plainText;
            }

            var searchResultModels = (await _themeRepository.GetListIncludeAsync())
                .Where(theme => theme!.Content != null && getPlainText(theme.Content).Contains(searchLine, StringComparison.OrdinalIgnoreCase))
                .SelectMany(theme => theme == null || theme.Content.IsNullOrEmpty() ? [] : GetSearchResultModels(theme, searchLine));

            var searchResultModelsWithIndexes = searchResultModels.GroupBy(s => new { s.ThemeId, s.SearchFragment })
                .SelectMany(group => group.Select((s, index) => 
                {
                    s.Index = index;
                    return s;
                }));

            return Ok(searchResultModelsWithIndexes);
        }

        private static IEnumerable<SearchResultModel> GetSearchResultModels(Theme theme, string searchLine)
        {
            const int offset = 100;

            var htmlPage = new HtmlDocument();
            htmlPage.LoadHtml(theme!.Content!);
            var rootNode = htmlPage.DocumentNode;
            var nodes = rootNode.ChildNodes.Where(x => WebUtility.HtmlDecode(x.InnerText).Contains(searchLine, StringComparison.OrdinalIgnoreCase));

            foreach (var node in nodes)
            {
                var childNode = node.ChildNodes.FirstOrDefault(x => x.Name != "#text" && WebUtility.HtmlDecode(x.InnerText).Contains(searchLine, StringComparison.OrdinalIgnoreCase)) ?? node;
                var content = childNode.InnerText;

                var lastIndex = content.Length;
                var startIndex = 0;

                while (startIndex < lastIndex)
                {
                    var index = content.IndexOf(WebUtility.HtmlEncode(searchLine), startIndex, StringComparison.OrdinalIgnoreCase);

                    if (index < 0)
                    {
                        break;
                    }

                    var leftOffset = offset;
                    var leftIndex = index - offset;

                    if (leftIndex < 0)
                    {
                        leftOffset = leftIndex + offset;
                        leftIndex = index - leftOffset;
                    }

                    var rightIndex = index + searchLine.Length + offset;
                    var rigthOffset = offset;

                    if (rightIndex > lastIndex)
                    {
                        rigthOffset = lastIndex - (index + searchLine.Length);
                    }

                    var searchFragmentText = content.Substring(leftIndex, leftOffset + searchLine.Length + rigthOffset);
                    var searchLineContent = content.Substring(index, searchLine.Length);
                    searchFragmentText = searchFragmentText.Replace(searchLineContent, $"<span style=\"background:yellow;color:black\">{searchLineContent.TrimStart('/')}</span>");
                    var searchFragment = content.Replace(childNode.InnerText, searchFragmentText);

                    var searchResultModel = new SearchResultModel
                    {
                        ChapterId = theme.ChapterId,
                        ThemeId = theme.ThemeId,
                        ThemeTitle = theme.ThemeTitle,
                        DateCreated = theme.DateCreated,
                        SearchFragment =
                            $"<{childNode.Name} style=\"{string.Join(';', childNode.Attributes.Select(attribute =>
                            $"{attribute.Name}:{attribute.DeEntitizeValue}"))}\">{searchFragment}</{childNode.Name}>",
                        Text = content
                    };

                    startIndex = index + searchLine.Length;

                    yield return searchResultModel;
                }
            }
        }
    }
}
