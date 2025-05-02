using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    [Index(nameof(UserID), nameof(SkillID))]
    public class UserQuestionHistory
    {
        [Key]
        [StringLength(36)]
        public string HistoryID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;

        [StringLength(36)]
        public string QuestionID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string SkillID { get; set; } = null!;

        [Required]
        public string? QuestionContent { get; set; }

        public string? UserAnswer { get; set; }

        public bool? IsCorrect { get; set; }

        public float? Score { get; set; }

        public int? TimeTaken { get; set; }

        [Required]
        public int DifficultyLevel { get; set; }

        public bool IsAIGenerated { get; set; } = false;

        public DateTime SubmitAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("SkillID")]
        public virtual StudentSkill Skill { get; set; } = null!;

        [ForeignKey("QuestionID")]
        public virtual QuizQuestion Question { get; set; } = null!;
    }
}
