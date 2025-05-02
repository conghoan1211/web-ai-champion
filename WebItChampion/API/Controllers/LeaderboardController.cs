using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : BaseController
    {
        private readonly ILeaderboardService _leaderboardService;
        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("GetTopLeaderboard")]
        public async Task<IActionResult> GetTopLeaderboard(string period, int page = 1, int pageSize = 20)
        {
            string userId = GetUserId();
            var (message, topList, currentUser) = await _leaderboardService.GetTopLeaderboard(userId, period, page, pageSize);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy danh sách thành công.", data = new { topList, currentUser } });
        }
    }
}
