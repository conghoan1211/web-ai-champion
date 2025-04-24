using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class SocialLike
    {
        [Key]
        [StringLength(36)]
        public string LikeID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string SocialPostID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string UserId { get; set; } = null!;

        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("SocialPostID")]
        public virtual SocialPost SocialPost { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
