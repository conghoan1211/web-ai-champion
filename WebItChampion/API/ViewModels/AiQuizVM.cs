using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class AiQuizVM
    {
    }

    // DTO cho dữ liệu nhận từ Gemini
    public class GeneratedQuestionVM
    {
        public string? SkillName { get; set; }
        public string? Question { get; set; }
        public List<string>? Options { get; set; } = new();
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
    }

    public class GenerateQuizBase
    {
        public string? UserId { get; set; }
        public int NumberQuestion { get; set; } = 10;

        [Description("Nhập tiêu đề bài quiz")]
        [Required(ErrorMessage = "Tiêu đề quiz không được để trống")]
        [StringLength(100, ErrorMessage = "Tiêu đề Quiz Không quá 100 ký tự")]
        public string Title { get; set; } = null!;

        [StringLength(300, ErrorMessage = "Mô tả Không quá 300 ký tự")]
        public string? Description { get; set; }
    }
    public class GenerateQuizFromSkills : GenerateQuizBase
    {
        public int Difficulty { get; set; } = 1;

        [Required(ErrorMessage = "Vui lòng chọn ít nhất 1 topic và kỹ năng")]
        public List<TopicSkillInput?> TopicSkills { get; set; } = null!;
    }

    public class GenerateQuizWeaknessSkills : GenerateQuizBase
    {
        public int Difficulty { get; set; } = 1;
    }

    public class GenerateQuizWrongAnswers : GenerateQuizBase
    {
        // Không cần thêm gì nếu không khác biệt
    }

    public class TopicSkillInput
    {
        public int? TopicId { get; set; }
        public List<string> Skills { get; set; } = new();
    }

}
