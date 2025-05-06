using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    [Index(nameof(TopicName), IsUnique = true)]
    public class Topic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TopicID { get; set; }

        [Required]
        [StringLength(100)]
        public string? TopicName { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<ExerciseCode> Exercises { get; set; } = new List<ExerciseCode>();
        public virtual ICollection<QuizTopic> QuizTopics { get; set; } = new List<QuizTopic>();
        public virtual ICollection<LearningResource> LearningResources { get; set; } = new List<LearningResource>();
        public virtual ICollection<StudentSkill> StudentSkills { get; set; } = new List<StudentSkill>();
    }
}
