using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    [Index(nameof(Period), nameof(PeriodStart), nameof(Score))]
    [Index(nameof(Period), nameof(UserID), nameof(PeriodStart))]
    public class Leaderboard
    {
        [Key]
        [StringLength(36)]
        public string LeaderboardID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;

        [Required]
        public float Score { get; set; }

        [Required]
        public int Rank { get; set; }
  
        [Required]
        [StringLength(50)]
        public string? Period { get; set; }

        public int TotalQuizzes { get; set; }
        public float AccuracyRate { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }  
    }
}
