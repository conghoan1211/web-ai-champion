using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    [Index(nameof(QuizID), nameof(UserID))]
    public class QuizSubmission
    {
        [Key]
        [StringLength(36)]
        public string QuizSubmissionID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string QuizID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string QuestionID { get; set; } = null!;

        [Required]
        public string? UserAnswer { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        public float? Score { get; set; }

        public DateTime SubmitAt { get; set; } = DateTime.Now;

        public string? Feedback { get; set; }

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("QuizID")]
        public virtual Quiz Quiz { get; set; } = null!;

        [ForeignKey("QuestionID")]
        public virtual QuizQuestion Question { get; set; } = null!;
    }
}
