using API.Configurations;
using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using API.Common;
using System.Security.Cryptography.Xml;
using System.Collections.Generic;

namespace API.Services
{
    public interface IAIQuizGeneratorService
    {
        public Task<(string, List<GeneratedQuestionVM>?)> GenerateQuizFromSkills(GenerateQuizFromSkills input);
        public Task<(string, List<GeneratedQuestionVM>?)> GenerateQuizFromWeakSkills(GenerateQuizWeaknessSkills input);
        public Task<(string, List<GeneratedQuestionVM>?)> GenerateQuizFromWrongAnswers(string userId, int numberQuestion, string title, string? description = null);
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

        private readonly string GenerateQuizFromSkillsPrompt =
                    @"Tạo một bài kiểm tra Python trắc nghiệm tiếng Việt gồm {0} câu hỏi dựa trên các chủ đề và kỹ năng tương ứng sau:
                    {1}
                    Lưu ý: Giữ nguyên tên kỹ năng như đã cho, không được thay đổi, trả về JSON hợp lệ, không có markdown nào, không có *.
                    Độ khó của câu hỏi: {2} .với mức độ khó tăng dần và mức độ (2) chiếm 80%,
                    Định dạng: Danh sách JSON với các trường: SkillName, Question, 4 Options (list), CorrectAnswer, Explanation."
;

        #endregion

        public async Task<(string, List<GeneratedQuestionVM>?)> GenerateQuizFromSkills(GenerateQuizFromSkills input)
        {
            // Validate inputs
            if (input.NumberQuestion <= 0 || input.NumberQuestion > 60)
                return ("Số lượng câu hỏi phải từ 1 đến 60", null);
            if (string.IsNullOrEmpty(input.UserId))
                return ("UserID is required", null);
            if (input.TopicSkills == null || !input.TopicSkills.Any())
                return ("Phải chọn ít nhất 1 topic và kỹ năng", null);

            // Gom tất cả skill name từ các topic
            var allSkillNames = input.TopicSkills.SelectMany(ts => ts.Skills).Distinct().ToList();
            if (allSkillNames.Count > input.NumberQuestion) return ("Tổng số kỹ năng không được vượt quá số câu hỏi", null);

            // Lấy tất cả kỹ năng hợp lệ từ DB
            var validSkills = await _context.StudentSkills.Include(s => s.Topic).Where(s => allSkillNames.Contains(s.SkillName)).ToListAsync();

            // Xác minh từng topic có đủ kỹ năng
            var invalidSkills = new List<string>();
            foreach (var topic in input.TopicSkills)
            {
                var skillsInTopic = validSkills
                    .Where(s => s.TopicID == topic.TopicId)
                    .Select(s => s.SkillName)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var notFound = topic.Skills.Where(s => !skillsInTopic.Contains(s)).ToList();
                if (notFound.Any()) invalidSkills.AddRange(notFound.Select(s => $"{s} (TopicID: {topic.TopicId})"));
            }
            if (invalidSkills.Any())
                return ($"Các kỹ năng không hợp lệ hoặc không thuộc topic tương ứng: {string.Join(", ", invalidSkills)}", null);

            bool quizExists = await _context.Quizzes.AnyAsync(q => q.Title == input.Title);
            if (quizExists) return ($"Tiêu đề quiz đã tồn tại. Vui lòng nhập tiêu đề khác.", null);

            var topicSkillText = string.Join("\n", input.TopicSkills.Select(ts =>
            {
                var topicName = validSkills.FirstOrDefault(t => t.TopicID == ts.TopicId)?.Topic.TopicName ?? $"ID {ts.TopicId}";
                return $"- Chủ đề {topicName}: {string.Join(", ", ts.Skills)}";
            }));

            var prompt = string.Format(GenerateQuizFromSkillsPrompt,
                input.NumberQuestion,
                topicSkillText,
                ((DifficultyLevel)input.Difficulty).ToString());

            var generatedQuiz = await CallGeminiApiAsync(prompt);
            if (generatedQuiz == null || !generatedQuiz.Any())
                return ("AI Gemini API không trả về câu hỏi phù hợp.", null);

            List<int?> topicIds = input.TopicSkills.Select(ts => ts.TopicId).Distinct().ToList();
            var msg = await SaveQuizToDatabaseAsync(input.Title, input.Description, generatedQuiz,
                topicIds, input.UserId,
                ((DifficultyLevel)input.Difficulty).ToString(), validSkills);
            if (msg.Length > 0) return (msg, null);

            return ("", generatedQuiz);
        }

        public async Task<(string, List<GeneratedQuestionVM>?)> GenerateQuizFromWrongAnswers(string userId, int numberQuestion, string title, string? description = null)
        {
            if (string.IsNullOrEmpty(userId))
                return ("User ID is required.", null);

            if (numberQuestion <= 0 || numberQuestion > 60)
                return ("Số lượng câu hỏi phải trong khoảng 1 đến 60.", null);

            bool quizExists = await _context.Quizzes.AnyAsync(q => q.Title == title);
            if (quizExists) return ($"Tiêu đề quiz đã tồn tại. Vui lòng nhập tiêu đề khác.", null);

            // Lấy các câu trả lời sai chưa từng làm đúng
            var wrongAnswers = await _context.QuestionHistories
              .Include(s => s.Question)
              .ThenInclude(q => q.Skill)
              .Where(qh => qh.UserID == userId && qh.IsCorrect == false)
              .Where(qh => !_context.QuestionHistories.Any(qh2 =>
                  qh2.UserID == userId &&
                  qh2.QuestionID == qh.QuestionID &&
                  qh2.IsCorrect == true))
              .Take(numberQuestion)
              .OrderByDescending(qh => qh.SubmitAt)
              .ToListAsync();

            if (!wrongAnswers.Any())
                return ("Không có câu trả lời sai để tạo quiz.", null);

            var generatedQuiz = wrongAnswers.Select(q => new GeneratedQuestionVM
            {
                SkillName = q.Skill.SkillName,
                Question = q.QuestionContent,
                Options = JsonConvert.DeserializeObject<List<string>?>(q.Question.Options),
                CorrectAnswer = q.Question.CorrectAnswer,
                Explanation = q.Question.Explanation
            }).ToList();

            // Lấy danh sách kỹ năng hợp lệ
            var skillNames = generatedQuiz.Select(g => g.SkillName).Distinct().ToList();
            var validSkills = await _context.StudentSkills
                .Include(s => s.Topic)
                .Where(s => skillNames.Contains(s.SkillName))
                .ToListAsync();

            List<int?> topicIds = validSkills.Select(s => s.TopicID).Distinct().ToList();

            var msg = await SaveQuizToDatabaseAsync(
                  title, description ?? "Quiz luyện tập từ câu sai",
                  generatedQuiz, topicIds, userId,
                  "Medium", validSkills);

            if (!string.IsNullOrEmpty(msg))
                return (msg, null);

            return ("", generatedQuiz);
        }

        // 2. Generate Quiz dựa trên kỹ năng yếu
        public async Task<(string, List<GeneratedQuestionVM>?)> GenerateQuizFromWeakSkills(GenerateQuizWeaknessSkills input)
        {
            // Validate inputs
            if (input.NumberQuestion <= 0 || input.NumberQuestion > 60)
                return ("Number of questions must be greater than 0 and less than 60", null);
            if (string.IsNullOrEmpty(input.UserId))
                return ("UserID is required.", null);

            bool quizExists = await _context.Quizzes.AnyAsync(q => q.Title == input.Title);
            if (quizExists) return ($"Tiêu đề quiz đã tồn tại. Vui lòng nhập tiêu đề khác.", null);

            var weakSkills = _context.StudentSkillProgresses                // need to improve
                .Where(s => s.UserID == input.UserId &&
                    (s.ProficiencyScore <= 1600 || s.SkillLevel <= (int)SkillLevel.Intermediate || s.ProbabilityKnown < 0.65))
                .OrderBy(s => s.ProficiencyScore)
                .Take(5)
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
                return ("No weak skills found for user!", null);

            if (weakSkills.Count() > input.NumberQuestion)
                return ("Số câu hỏi cần nhiều hơn số lượng kĩ năng yếu.", null);

            var weakSkillsWithTopic = await _context.StudentSkills
                .Where(s => weakSkills.Select(ws => ws.SkillID).Contains(s.SkillID))
                .Include(s => s.Topic)
                .ToListAsync();

            var topicIds = weakSkillsWithTopic.Select(s => (int?)s.Topic.TopicID).Distinct().ToList();

            var topicSkillText = string.Join("\n", weakSkillsWithTopic.Select(ts =>
                 $"- Chủ đề {ts.Topic.TopicName}: {string.Join(", ", ts.SkillName)}"));

            var prompt = string.Format(GenerateQuizFromSkillsPrompt,
                   input.NumberQuestion,
                   topicSkillText,
                   ((DifficultyLevel)input.Difficulty).ToString());

            var generatedQuiz = await CallGeminiApiAsync(prompt);

            if (generatedQuiz == null || !generatedQuiz.Any())
                throw new Exception("Gemini API didn't return quiz questions.");

            var msg = await SaveQuizToDatabaseAsync(input.Title, input.Description, generatedQuiz,
                topicIds, input.UserId,
                ((DifficultyLevel)input.Difficulty).ToString(), weakSkillsWithTopic);
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
            List<GeneratedQuestionVM> questions, List<int?> topicIds, string userId,
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
                    Description = description,
                    DifficultyLevel = Enum.TryParse<DifficultyLevel>(difficulty, true, out var diffEnum) ? (int)diffEnum : 1,
                    TimeLimit = questions.Count > 0 ? questions.Count + 5 : 0,
                    CreateAt = DateTime.UtcNow,
                    Status = (int)QuizStatus.Private,
                };
                _context.Quizzes.Add(quiz);

                // Lưu nhiều topic
                var quizTopics = topicIds.Select((topicId, index) => new QuizTopic
                {
                    QuizTopicID = Guid.NewGuid().ToString(),
                    QuizID = quizId,
                    TopicID = (int)topicId,
                    IsPrimaryTopic = index == 0, 
                    CreatedAt = DateTime.UtcNow
                }).ToList();
                _context.QuizTopics.AddRange(quizTopics);

                var quizQuestions = new List<QuizQuestion>();
                foreach (var q in questions)
                {
                    var matchedSkill = validSkills.FirstOrDefault(s => Converter.Normalize(s.SkillName) == Converter.Normalize(q.SkillName));
                    if (matchedSkill == null)
                    {
                        Console.WriteLine($"Không tìm thấy skill: {q.SkillName}");
                        continue;
                    }
                    quizQuestions.Add(new QuizQuestion
                    {
                        QuestionID = Guid.NewGuid().ToString(),
                        QuizID = quizId,
                        SkillID = matchedSkill.SkillID,
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
    }
}
