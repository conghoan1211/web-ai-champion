using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Notification
    {
        [Key]
        [StringLength(36)]
        public string NotificationID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string? Content { get; set; }

        [Required]
        public int Type { get; set; }

        [StringLength(36)]
        public string? RelatedID { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;
    }
}
