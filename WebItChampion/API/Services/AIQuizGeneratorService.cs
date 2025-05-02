using API.Configurations;
using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using API.Common;

namespace API.Services
{
    public interface IAIQuizGeneratorService
    {
        public Task<(string, List<GeneratedQuestionVM>?)> GenerateQuizFromSkills(GenerateQuizRequest input);
        public Task<string> GenerateQuizFromWeakSkillsAsync(string userId, int numberQuestion, string difficulty = "medium");

    }

    public class AIQuizGeneratorService : IAIQuizGeneratorService
    {
        private readonly Sep490Context _context;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public AIQuizGeneratorService(Sep490Context context, HttpClient httpClient, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _httpClient = httpClient;
        }
        #region
        private readonly string AIApiKey = ConfigManager.gI().GeminiKey;
        private readonly string AIUri = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        private readonly string GeneratedQuizForNewUserPrompt =
                    @"Tạo một bài kiểm tra python trắc nghiệm tiếng Việt có {0} câu hỏi dựa trên chủ đề {1} theo các kĩ năng sau:
                    {2}
                    Độ khó của câu hỏi: {3} .với mức độ khó tăng dần và mức độ (3) chiếm 80%,
                    Định dạng: Danh sách JSON với các trường: SkillName, Question, 4 Options (list), CorrectAnswer, Explanation.";

        #endregion

        public async Task<(string, List<GeneratedQuestionVM>?)> GenerateQuizFromSkills(GenerateQuizRequest input)
        {
            // Validate inputs
            if (input.NumberQuestion <= 0 || input.NumberQuestion > 60)
                throw new Exception("Number of questions must be greater than 0 and less than 60");
            if (string.IsNullOrEmpty(input.UserId))
                throw new Exception("UserID is required.");
            if (input.Skills == null || !input.Skills.Any())
                throw new Exception("Skills list cannot be empty.");
            if (input.Skills.Count > input.NumberQuestion)
                throw new Exception("Number of skills cannot exceed number of questions.");

            // Validate skills and their topics
            var validSkills = await _context.StudentSkills.Include(s => s.Topic).Where(s => input.Skills.Contains(s.SkillName) && s.TopicID == input.TopicId).ToListAsync();
            if (validSkills.Count != input.Skills.Count)
            {
                var notFoundSkills = input.Skills.Except(validSkills.Select(s => s.SkillName)).ToList();
                return ($"Invalid skill: {string.Join(", ", notFoundSkills)} does not exist or does not belong to TopicID {input.TopicId}.", null);
            }
            bool quizExists = await _context.Quizzes.AnyAsync(q => q.Title == input.Title);
            if (quizExists) return ($"Tiêu đề quiz đã tồn tại. Vui lòng nhập tiêu đề khác.", null);

            var prompt = string.Format(GeneratedQuizForNewUserPrompt,
                input.NumberQuestion,
                validSkills.First().Topic.TopicName,
                string.Join(", ", input.Skills).Trim(),
                input.Difficulty);

            var generatedQuiz = await CallGeminiApiAsync(prompt);
            if (generatedQuiz == null || !generatedQuiz.Any())
                return ("AI Gemini API didn't return quiz questions.", null);

            var msg = await SaveQuizToDatabaseAsync(input.Title, input.Description, generatedQuiz,
                input.TopicId, input.UserId,
                input.Difficulty, validSkills);
            if (msg.Length > 0) return (msg, null);

            return ("", generatedQuiz);
        }


        private async Task<List<GeneratedQuestionVM>?> CallGeminiApiAsync(string prompt)
        {
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } }
            };

            var jsonPayload = JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented // Cho dễ đọc
            });

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{AIUri}?key={AIApiKey}", content);
            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Gemini raw response: " + responseString);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Gemini API error: {response.StatusCode} - {responseString}");

            dynamic responseData = JsonConvert.DeserializeObject(responseString);
            if (responseData?.candidates == null || responseData.candidates.Count == 0)
                throw new Exception("Lỗi khi gọi API AI: Không nhận được phản hồi hợp lệ.");

            string aiResponse = responseData.candidates[0].content.parts[0].text;
            aiResponse = Converter.SanitizeJsonString(aiResponse); // Làm sạch chuỗi trước khi Deserialize

            return JsonConvert.DeserializeObject<List<GeneratedQuestionVM>>(aiResponse);
        }

        // Helper: Save Quiz + Questions vào Database
        private async Task<string> SaveQuizToDatabaseAsync(string title, string description,
            List<GeneratedQuestionVM> questions, int topicId, string userId,
            string difficulty, List<StudentSkill> validSkills)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(); // Mở transaction
            try
            {
                var quizId = Guid.NewGuid().ToString();
                var quiz = new Quiz
                {
                    QuizID = quizId,
                    Title = title.Trim(),
                    UserID = userId,
                    TopicID = topicId,
                    Description = description,
                    DifficultyLevel = Enum.TryParse<DifficultyLevel>(difficulty, true, out var diffEnum) ? (int)diffEnum : 0,
                    TimeLimit = questions.Count > 0 ? questions.Count + 5 : 0,
                    CreateAt = DateTime.UtcNow,
                };
                _context.Quizzes.Add(quiz);

                var quizQuestions = new List<QuizQuestion>();
                var addedSkillIds = new HashSet<string>();

                foreach (var q in questions)
                {
                    var matchedSkill = validSkills.FirstOrDefault(s => s.SkillName.Equals(q.SkillName, StringComparison.OrdinalIgnoreCase));
                    var skillId = matchedSkill.SkillID;

                    quizQuestions.Add(new QuizQuestion
                    {
                        QuestionID = Guid.NewGuid().ToString(),
                        QuizID = quizId,
                        SkillID = skillId,
                        Content = q.Question,
                        QuestionType = (int)QuestionType.Essay,
                        Options = JsonConvert.SerializeObject(q.Options),
                        CorrectAnswer = q.CorrectAnswer,
                        Explanation = q.Explanation,
                        DifficultyLevel = (int)diffEnum,
                        CreateAt = DateTime.UtcNow,
                        IsAICreated = true
                    });
                }
                _context.QuizQuestions.AddRange(quizQuestions);

                // Link to class if specified
                //if (!string.IsNullOrEmpty(classId))
                //{
                //    var classQuiz = new ClassQuiz
                //    {
                //        ClassQuizID = Guid.NewGuid().ToString(),
                //        ClassID = classId,
                //        QuizID = quiz.QuizID,
                //        CreateAt = DateTime.UtcNow
                //    };
                //    _context.ClassQuizzes.Add(classQuiz);
                //}
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ($"Lỗi : {ex.Message} \n - {ex.InnerException}");
            }
        }

        // 2. Generate Quiz dựa trên kỹ năng yếu
        public async Task<string> GenerateQuizFromWeakSkillsAsync(string userId, int numberQuestion, string difficulty = "medium")
        {
            var weakSkills = _context.StudentSkillProgresses
                .Where(p => p.UserID == userId && (p.ProficiencyScore < 70 || p.ProbabilityKnown < 0.7))
                .Join(_context.StudentSkills,
                      progress => progress.SkillID,
                      skill => skill.SkillID,
                      (progress, skill) => new
                      {
                          skill.SkillID,
                          skill.SkillName,
                          skill.Description
                      })
                .ToList();

            if (!weakSkills.Any())
                throw new Exception("No weak skills found for user!");

            var SkillList = string.Join("\n", weakSkills.Select(x => $"- {x.SkillName}: {x.Description}"));

            var prompt = string.Format(GeneratedQuizForNewUserPrompt, SkillList.Trim(), difficulty);

            var generatedQuiz = await CallGeminiApiAsync(prompt);

            if (generatedQuiz == null || !generatedQuiz.Any())
                throw new Exception("Gemini API didn't return quiz questions.");

            //var message = await SaveQuizToDatabaseAsync("Skill Improvement Quiz", "Quiz based on weak skills", generatedQuiz);

            return "message";
        }

    }
}
