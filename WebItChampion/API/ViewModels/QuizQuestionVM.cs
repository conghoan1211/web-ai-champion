namespace API.ViewModels
{
    public class QuizQuestionVM
    {
        public string QuestionID { get; set; } = null!;
        public string QuizID { get; set; } = null!;
        public string? Content { get; set; }
        public int QuestionType { get; set; }
        public string? Options { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public int DifficultyLevel { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public bool IsAICreated { get; set; } = false;
    }
}
