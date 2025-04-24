using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ClassQuiz
    {
        [Key]
        [StringLength(36)]
        public string ClassQuizID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string ClassID { get; set; } = null!;

        [Required]
        [StringLength(36)]
        public string QuizID { get; set; } = null!;

        public DateTime CreateAt { get; set; } = DateTime.Now;

        [ForeignKey("ClassID")]
        public virtual Class Class { get; set; } = new Class();

        [ForeignKey("QuizID")]
        public virtual Quiz Quiz { get; set; } = new Quiz();

    }
}
