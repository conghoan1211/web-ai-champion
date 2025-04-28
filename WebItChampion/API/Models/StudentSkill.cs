using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    [Index(nameof(SkillName), nameof(TopicID), IsUnique = true)]
    [Index(nameof(SkillID), nameof(TopicID))]
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

        [DefaultValue(false)]
        public bool IsCoreSkill { get; set; } = false;             

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt { get; set; }

        // Navigation properties
        [ForeignKey("TopicID")]
        public virtual Topic Topic { get; set; } = null!;
        public virtual ICollection<StudentSkillProgress> SkillProgresses { get; set; } = new List<StudentSkillProgress>();
        public virtual ICollection<UserQuestionHistory> QuestionHistories { get; set; } = new List<UserQuestionHistory>();

    }
}
