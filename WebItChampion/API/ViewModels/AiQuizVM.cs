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
        public List<string> Options { get; set; } = new List<string>();
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
    }

    public class GenerateQuizRequest
    {
        public string UserId { get; set; } = null!;
        public int NumberQuestion { get; set; } = 10;
        public string Difficulty { get; set; } = "easy";
        public List<string> Skills { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng thêm chủ đề.")]
        public int TopicId { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(100, ErrorMessage = "Tiêu đề Quiz Không quá 100 ký tự")]
        public string Title { get; set; } = null!;

        [StringLength(300, ErrorMessage = "Mô tả Không quá 300 ký tự")]
        public string Description { get; set; } = null!;
    }
}
