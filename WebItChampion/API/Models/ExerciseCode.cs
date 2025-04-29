using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ExerciseCode
    {
        [Key]
        [StringLength(36)]
        public string ExerciseID { get; set; } = null!;
        [Required]
        [StringLength(255)]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public int DifficultyLevel { get; set; }
        public int? TopicID { get; set; }
        public string? InputFormat { get; set; }
        public string? OutputFormat { get; set; }
        public string? SampleInput { get; set; }
        public string? SampleOutput { get; set; }
        public string? TestCases { get; set; }
        public int? MaxTimeLimit { get; set; }
        public int? MaxMemoryLimit { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; }

        // Navigation properties
        [ForeignKey("TopicID")]
        public virtual Topic? Topic { get; set; }
        public virtual ICollection<SubmissionCode> Submissions { get; set; } = new List<SubmissionCode>();
        public virtual ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();
     //   public virtual ICollection<LearningPathItem> LearningPathItems { get; set; } = new List<LearningPathItem>();
    }
}
