using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ClassMembership
    {
        [Key]
        [StringLength(36)]
        public string MembershipID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string ClassID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string StudentID { get; set; } = null!;

        public DateTime JoinDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("ClassID")]
        public virtual Class Class { get; set; } = null!;

        [ForeignKey("StudentID")]
        public virtual User Student { get; set; } = new User();
    }
}
