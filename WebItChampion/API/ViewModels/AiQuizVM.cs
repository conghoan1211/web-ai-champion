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
        public int NumberQuestion { get; set; }
        public string Difficulty { get; set; } = "easy";
        public string UserId { get; set; } = null!;
        public List<string> Skills { get; set; } = null!;
        public int TopicId { get; set; }
    }
}
