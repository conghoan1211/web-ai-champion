using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class SubmissionCode
    {
        [Key]
        [StringLength(36)]
        public string SubmissionID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string ExerciseID { get; set; } = null!;

        [Required]
        public string? Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Language { get; set; } = "Python";

        [Required]
        public int Status { get; set; }

        public float? Score { get; set; }

        public int? ExecutionTime { get; set; }
        public int? MemoryUsed { get; set; }

        public string? ErrorMessage { get; set; }

        public string? Feedback { get; set; }

        public DateTime SubmitAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("ExerciseID")]
        public virtual ExerciseCode Exercise { get; set; } = null!;
    }
}
