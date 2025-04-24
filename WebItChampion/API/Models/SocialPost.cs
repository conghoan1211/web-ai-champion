using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class SocialPost
    {
        [Key]
        [StringLength(36)]
        public string SocialPostID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;

        [Required]
        public string? Content { get; set; }

        [StringLength(255)]
        public string? MediaURL { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt { get; set; }

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;
        public virtual ICollection<SocialComment> Comments { get; set; } = new List<SocialComment>();
        public virtual ICollection<SocialLike> Likes { get; set; } = new List<SocialLike>();
    }
}
