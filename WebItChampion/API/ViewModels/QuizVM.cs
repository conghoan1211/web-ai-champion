using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class QuizVM
    {
        public string QuizID { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int DifficultyLevel { get; set; }
        public int? TopicID { get; set; }
        public int? TimeLimit { get; set; }
        public int Status { get; set; } = 1; // 0: Private, 1: Public, 2: Hide, 3: Deleted 
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; }
        public string UserID { get; set; } = null!;  // Ai tạo quiz này
    }
}
