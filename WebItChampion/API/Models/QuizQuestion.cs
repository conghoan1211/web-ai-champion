using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class QuizQuestion
    {
        [Key]
        [StringLength(36)]
        public string QuestionID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string QuizID { get; set; } = null!;

        [Required]
        public string? Content { get; set; }

        [Required]
        public int QuestionType { get; set; }

        public string? Options { get; set; }

        [Required]
        public string? CorrectAnswer { get; set; }

        public string? Explanation { get; set; }

        [Required]
        public int DifficultyLevel { get; set; }

        [Column("CreateAt")]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [Column("IsAICreated")]
        public bool IsAICreated { get; set; } = false;

        // Navigation properties
        [ForeignKey("QuizID")]
        public virtual Quiz Quiz { get; set; } = null!;
        public virtual ICollection<QuizSubmission> Submissions { get; set; } = new List<QuizSubmission>();
        public virtual ICollection<UserQuestionHistory> QuestionHistories { get; set; } = new List<UserQuestionHistory>();
    }
}
