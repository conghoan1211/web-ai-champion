using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class QuizTopic
    {
        [Key]
        public string? QuizTopicID { get; set; } = null!;

        [StringLength(36)]
        public string QuizID { get; set; } = null!;
        public int TopicID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsPrimaryTopic { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
        
        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
        public string? Notes { get; set; }

        public Quiz Quiz { get; set; } = null!;
        public Topic Topic { get; set; } = null!;
    }

}
