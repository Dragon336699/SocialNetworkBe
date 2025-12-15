using Domain.Contracts.Requests.Search;
using Domain.Contracts.Responses.Search;
using Domain.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain.Enum.Search.Types;

namespace SocialNetworkBe.Controllers
{
    [ApiController]
    [Route("api/v1/search")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string keyword,
            [FromQuery] SearchType? type = SearchType.All,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new SearchResponse
                    {
                        Message = "Keyword is required",
                        Results = null
                    });
                }

                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var request = new SearchRequest
                {
                    Keyword = keyword,
                    Type = type,
                    Skip = skip,
                    Take = take
                };

                var results = await _searchService.SearchAsync(request, userId, false);

                if (results == null)
                {
                    return StatusCode(500, new SearchResponse
                    {
                        Message = "Search failed",
                        Results = null
                    });
                }

                return Ok(new SearchResponse
                {
                    Message = "Search completed successfully",
                    Results = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new SearchResponse
                {
                    Message = ex.Message,
                    Results = null
                });
            }
        }

        [HttpPost("save-history")]
        public async Task<IActionResult> SaveSearchHistory([FromBody] SaveSearchHistoryRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return BadRequest(new { message = "Content is required" });
                }

                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _searchService.SaveSearchHistoryAsync(userId, request);

                if (!result)
                {
                    return StatusCode(500, new { message = "Failed to save search history" });
                }

                return Ok(new { message = "Search history saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentSearches([FromQuery] int take = 10)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var recentSearches = await _searchService.GetRecentSearchesAsync(userId, take);

                if (recentSearches == null)
                {
                    return Ok(new { message = "No recent searches", data = new List<object>() });
                }

                return Ok(new { message = "Get recent searches successfully", data = recentSearches });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("history/{historyId}")]
        public async Task<IActionResult> DeleteSearchHistory(Guid historyId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _searchService.DeleteSearchHistoryAsync(userId, historyId);

                if (!result)
                {
                    return NotFound(new { message = "Search history not found" });
                }

                return Ok(new { message = "Delete search history successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("history/clear")]
        public async Task<IActionResult> ClearAllSearchHistory()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _searchService.ClearAllSearchHistoryAsync(userId);

                return Ok(new { message = "Clear all search history successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
