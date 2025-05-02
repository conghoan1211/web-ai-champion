using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class QuizSubmissionVM
    {
    }

    // Data models 
    public class QuizSubmissionRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "QuizId is required.")]
        public string? QuizId { get; set; }
        public string? ClassId { get; set; }  
        
        [Required(ErrorMessage = "Answers are required.")]
        public List<AnswerInput>? Answers { get; set; }
    }

    public class AnswerInput
    {
        public string? QuestionId { get; set; }
        public string? UserAnswer { get; set; }
    }
}
