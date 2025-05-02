using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class LeaderboardVM
    {
        public string LeaderboardID { get; set; } = null!;
        public string UserID { get; set; } = null!;
        public string? Username { get; set; }
        public string? Avatar {  get; set; }
        public float Score { get; set; }
        public int Rank { get; set; }
        public string? Period { get; set; }
        public int TotalQuizzes { get; set; }
        public float AccuracyRate { get; set; }
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
