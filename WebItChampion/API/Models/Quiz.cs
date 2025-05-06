using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    [Index(nameof(Title), IsUnique = true)]
    public class Quiz
    {
        [Key]
        [StringLength(36)]
        public string QuizID { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        [Required]
        public int DifficultyLevel { get; set; }

        public int? TimeLimit { get; set; }

        public int Status { get; set; } = 1; // 0: Private, 1: Public, 2: Hide, 3: Deleted 

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt { get; set; }

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;  // Ai tạo quiz này

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        public virtual ICollection<QuizTopic> QuizTopics { get; set; } = new List<QuizTopic>();
        public virtual ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public virtual ICollection<QuizSubmission> Submissions { get; set; } = new List<QuizSubmission>();
        public virtual ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();
        public virtual ICollection<ClassQuiz> ClassQuizzes { get; set; } = new List<ClassQuiz>();
     //   public virtual ICollection<LearningPathItem> LearningPathItems { get; set; }
    }
}
