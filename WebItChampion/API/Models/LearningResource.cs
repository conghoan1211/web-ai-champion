using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class LearningResource
    {
        [Key]
        [StringLength(36)]
        public string ResourceID { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        [StringLength(255)]
        public string? ContentURL { get; set; }

        public int? TopicID { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt { get; set; }

        // Navigation properties
        [ForeignKey("TopicID")]
        public virtual Topic? Topic { get; set; } 
      //  public virtual ICollection<LearningPathItem> LearningPathItems { get; set; }
    }
}
