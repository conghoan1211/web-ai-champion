using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class StudentSkill
    {
        [Key]
        [StringLength(36)]
        public string SkillID { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string? SkillName { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public int? TopicID { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt { get; set; }

        // Navigation properties
        [ForeignKey("TopicID")]
        public virtual Topic Topic { get; set; } = null!;
        public virtual ICollection<StudentSkillProgress> SkillProgresses { get; set; } = new List<StudentSkillProgress>();
        public virtual ICollection<UserQuestionHistory> QuestionHistories { get; set; } = new List<UserQuestionHistory>();

    }
}
