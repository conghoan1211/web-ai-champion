using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Class
    {
        [Key]
        [StringLength(36)]
        public string ClassID { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ClassName { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [StringLength(36)]
        public string TeacherID { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt { get; set; }

        // Navigation properties
        [ForeignKey("TeacherID")]
        public virtual User Teacher { get; set; } = null!;
        public virtual ICollection<ClassMembership> Memberships { get; set; } = new List<ClassMembership>();
        public virtual ICollection<ClassQuiz> ClassQuizzes { get; set; } = new List<ClassQuiz>();
    }
}
