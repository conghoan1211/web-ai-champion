using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Quiz
    {
        [Key]
        [StringLength(36)]
        public string QuizID { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int DifficultyLevel { get; set; }

        public int? TopicID { get; set; }

        public int? TimeLimit { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt { get; set; }

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;  // Ai tạo quiz này

        // Navigation properties
        [ForeignKey("TopicID")]
        public virtual Topic? Topic { get; set; }

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }  

        public virtual ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public virtual ICollection<QuizSubmission> Submissions { get; set; } = new List<QuizSubmission>();
        public virtual ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();
        public virtual ICollection<ClassQuiz> ClassQuizzes { get; set; } = new List<ClassQuiz>();
     //   public virtual ICollection<LearningPathItem> LearningPathItems { get; set; }
    }
}
