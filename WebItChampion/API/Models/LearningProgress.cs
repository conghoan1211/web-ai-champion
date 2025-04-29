using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class LearningProgress
    {
        [Key]
        [StringLength(36)]
        public string ProgressID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;

        [StringLength(36)]
        public string ExerciseID { get; set; } = null!;

        [StringLength(36)]
        public string QuizID { get; set; } = null!;

        public float? Score { get; set; }

        public int? TimeSpent { get; set; }

        public DateTime? CompletionDate { get; set; }

        [Required]
        public int Status { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } 

        [ForeignKey("ExerciseID")]
        public virtual ExerciseCode Exercise { get; set; } 

        [ForeignKey("QuizID")]
        public virtual Quiz Quiz { get; set; }  
    }
}
