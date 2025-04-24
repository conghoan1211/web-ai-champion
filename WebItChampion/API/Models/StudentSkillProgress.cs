using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class StudentSkillProgress
    {
        [Key]
        [StringLength(36)]
        public string ProgressID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string SkillID { get; set; } = null!;

        public float? ProficiencyScore { get; set; }

        public int? SkillLevel { get; set; }

        public float ProbabilityKnown { get; set; }

        public int Attempts { get; set; } = 0;

        public float? SuccessRate { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("SkillID")]
        public virtual StudentSkill Skill { get; set; } = null!;
    }
}
