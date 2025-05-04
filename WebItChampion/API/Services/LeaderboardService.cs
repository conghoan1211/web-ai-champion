using API.Models;
using API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public interface ILeaderboardService
    {
        public Task<(string, List<LeaderboardVM>?, LeaderboardVM?)> GetTopLeaderboard(string userId, string? period, int page = 1, int pageSize = 20);
    }
    public class LeaderboardService : ILeaderboardService
    {
        private readonly Sep490Context _context;
        public LeaderboardService(Sep490Context context)
        {
            _context = context;
        }
        public async Task<(string, List<LeaderboardVM>?, LeaderboardVM?)> GetTopLeaderboard(string userId, string? period, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(userId)) return ("User ID was not found", null, null);

            var now = DateTime.UtcNow.Date;
            DateTime? periodStart = period switch
            {
                "daily" => now,
                "weekly" => now.AddDays(-(int)now.DayOfWeek),
                "monthly" => new DateTime(now.Year, now.Month, 1),
                "all" => null,
                _ => null
            };

            int skip = (page - 1) * pageSize;
            var topList = await _context.Leaderboards.Include(x => x.User)
                .Where(l =>
                    (period == "all" && l.Period == "all") ||
                    (period != "all" && l.Period == period && l.PeriodStart == periodStart)
                ).OrderByDescending(l => l.Score)
                .ThenBy(l => l.UpdateAt)
                .Skip(skip)
                .Take(pageSize)
                .Select(l => new LeaderboardVM
                {
                    LeaderboardID = l.LeaderboardID,
                    UserID = l.UserID,
                    Avatar = l.User.Avatar,
                    Username = l.User.Username,
                    Score = l.Score,
                    Rank = l.Rank,
                    Period = l.Period,
                    UpdateAt = l.UpdateAt,
                    TotalQuizzes = l.TotalQuizzes,
                    AccuracyRate = l.AccuracyRate
                })
                .ToListAsync();

            var isInPage = topList.Any(x => x.UserID == userId);
            LeaderboardVM? currentUser = null;
            if (!isInPage)
            {
                currentUser = await _context.Leaderboards.Include(x => x.User)
                    .Where(l => (period == "all" && l.Period == "all") ||
                    (period != "all" && l.Period == period && l.PeriodStart == periodStart)
                           && l.UserID == userId)
                    .Select(l => new LeaderboardVM
                    {
                        Avatar = l.User.Avatar,
                        UserID = l.UserID,
                        LeaderboardID = l.LeaderboardID,
                        Username = l.User.Username,
                        Score = l.Score,
                        Rank = l.Rank,
                        Period = l.Period,
                        UpdateAt = l.UpdateAt,
                        TotalQuizzes = l.TotalQuizzes,
                        AccuracyRate = l.AccuracyRate
                    })
                    .FirstOrDefaultAsync();
            }
            return ("", topList, currentUser);
        }

    }
}
