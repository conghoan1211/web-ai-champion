using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class SocialComment
    {
        [Key]
        [StringLength(36)]
        public string CommentID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string SocialPostID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;

        [Required]
        public string? Content { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("SocialPostID")]
        public virtual SocialPost SocialPost { get; set; } = null!;

        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;
    }
}
