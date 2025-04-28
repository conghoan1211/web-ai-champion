using API.Configurations;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace API.Services
{
    public interface ISkillGeneratorService
    {
        public Task<(string, List<StudentSkillVM>?)> GenerateSkillFromAnswers(string studentId);
        public Task<(string, List<StudentSkillVM>?)> GenerateSkillFromTopic(int topicId, string studentId, int numberSkill);
    }
    public class SkillGeneratorService : ISkillGeneratorService
    {
        private readonly Sep490Context _context;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public SkillGeneratorService(Sep490Context context, HttpClient httpClient, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        #region Prompts and config for Gemini AI
        private readonly string AIApiKey = ConfigManager.gI().GeminiKey;
        private readonly string AIUri = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        public readonly string GenerateSkillFromIncorrectAnswersPrompt =
                    @"Generate a new programming Python skill based on a student's incorrect quiz answers.
                    Context:
                    - Current skills: {0}
                    - Topics: {1}
                    - Incorrect answers: {2}
                    Requirements:
                    - Skill must be specific, actionable, and related to programming (e.g., 'List Comprehension' or 'Binary Search Optimization').
                    - Provide:
                      - SkillName: A concise name (max 100 characters)
                      - Description: A brief explanation (max 255 characters)
                      - TopicName: The most relevant topic from the provided list
                    Avoid duplicating existing skills: {3}.";

        public readonly string GenerateSkillForTopicPrompt =
                    @"Tạo {0} kỹ năng lập trình Python mới (dễ đến trung bình) cho chủ đề '{1}'.
                    Yêu cầu:
                    - Kỹ năng phải cụ thể, có thể thực hiện được và liên quan đến lập trình python.
                    - Xuất chính xác đối tượng JSON này, không có bất kỳ dấu ngoặc kép, dấu ngoặc kép ngược hoặc văn bản bổ sung nào:
                         - SkillName: A concise name (max 100 characters)
                         - Description: A brief explanation (max 255 characters)
                         - TopicName: Must be '{1}'
                    - Đảm bảo JSON được mã hóa UTF-8 chính xác mà không làm hỏng các ký tự không phải ASCII (như tiếng Việt).
                    - Tránh trùng lặp các kỹ năng hiện có: {2}.";
        #endregion


        public async Task<(string, List<StudentSkillVM>?)> GenerateSkillFromAnswers(string studentId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(); // Mở transaction
            try
            {
                var incorrectAnswers = await _context.QuestionHistories
                    .Where(qh => qh.UserID == studentId && qh.IsCorrect == false)
                    .Include(qh => qh.Skill)
                    .OrderByDescending(qh => qh.SubmitAt)
                    .Take(3)
                    .ToListAsync();

                if (!incorrectAnswers.Any())
                    return ("No incorrect answers found to generate a new skill.", null);

                var skillNames = incorrectAnswers.Select(qh => qh.Skill?.SkillName ?? "Unknown").ToList();
                var topicIds = incorrectAnswers.Select(qh => qh.Skill?.TopicID).Distinct().ToList();
                var topics = await _context.Topics
                    .Where(t => topicIds.Contains(t.TopicID))
                    .Select(t => t.TopicName)
                    .ToListAsync();
                var answerDetails = incorrectAnswers.Select(qh => $"Question: {qh.QuestionContent}, Answer: {qh.UserAnswer}").ToList();
                var existingSkillNames = await _context.StudentSkills.Select(s => s.SkillName).ToListAsync();

                var prompt = string.Format(
                    GenerateSkillFromIncorrectAnswersPrompt,
                    string.Join(", ", skillNames),
                    string.Join(", ", topics),
                    string.Join("; ", answerDetails),
                    string.Join(", ", existingSkillNames)
                );
                var newSkills = await CallGeminiApiAsync(prompt);
                if (newSkills == null || !newSkills.Any())
                    return ("Failed to generate valid skills.", null);

                var studentSkillVMs = new List<StudentSkillVM>();
                foreach (var newSkill in newSkills)
                {
                    // Kiểm tra nếu skill đã tồn tại
                    var existingSkill = existingSkillNames.FirstOrDefault(s => s.ToLower() == newSkill.SkillName.ToLower());
                    if (existingSkill != null)
                        continue; // Nếu đã có skill, bỏ qua

                    // Kiểm tra và tạo mới topic nếu cần
                    var topic = await _context.Topics
                        .FirstOrDefaultAsync(t => t.TopicName.ToLower() == newSkill.TopicName.ToLower());
                    if (topic == null)
                    {
                        topic = new Topic
                        {
                            TopicName = newSkill.TopicName,
                            Description = $"Auto-generated topic for {newSkill.SkillName}",
                            IsActive = true,
                            IsDeleted = false
                        };
                        _context.Topics.Add(topic);
                    }

                    // Tạo skill mới và thêm vào DB
                    var skill = new StudentSkill
                    {
                        SkillID = Guid.NewGuid().ToString(),
                        SkillName = newSkill.SkillName,
                        Description = newSkill.Description,
                        TopicID = topic.TopicID,
                        IsCoreSkill = false,
                        CreateAt = DateTime.UtcNow
                    };
                    _context.StudentSkills.Add(skill);

                    // Tạo progress cho học sinh với skill mới
                    var progress = new StudentSkillProgress
                    {
                        ProgressID = Guid.NewGuid().ToString(),
                        UserID = studentId,
                        SkillID = skill.SkillID,
                        ProficiencyScore = 0,
                        SkillLevel = 1,
                        ProbabilityKnown = 0,
                        LastUpdated = DateTime.UtcNow,
                        Attempts = 0,
                        SuccessRate = 0
                    };
                    _context.StudentSkillProgresses.Add(progress);

                    // Thêm vào danh sách để trả về
                    var mapper = _mapper.Map<StudentSkillVM>(skill);
                    studentSkillVMs.Add(mapper);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (string.Empty, studentSkillVMs);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ($"Failed to generate skill from answers: {ex.Message}", null);
            }
        }

        public async Task<(string, List<StudentSkillVM>?)> GenerateSkillFromTopic(int topicId, string studentId, int numberSkill)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();  
            try
            {
                var topic = await _context.Topics.FindAsync(topicId);
                if (topic == null) return ("Topic not found.", null);

                var existingSkills = await _context.StudentSkills
                    .Where(s => s.TopicID == topicId)
                    .Select(s => s.SkillName)
                    .ToListAsync();

                var prompt = string.Format(GenerateSkillForTopicPrompt, numberSkill, topic.TopicName, string.Join(", ", existingSkills));
        
                var newSkills = await CallGeminiApiAsync(prompt);
                if (newSkills == null || !newSkills.Any())
                    return ("Failed to generate any valid skills.", null);

                foreach (var newSkill in newSkills)
                {
                    var existingSkill = await _context.StudentSkills
                        .FirstOrDefaultAsync(s => s.SkillName.ToLower() == newSkill.SkillName.ToLower());

                    if (existingSkill != null)
                        continue; // Skip nếu skill đã tồn tại

                    var skill = new StudentSkill
                    {
                        SkillID = Guid.NewGuid().ToString(),
                        SkillName = newSkill.SkillName,
                        Description = newSkill.Description,
                        TopicID = topicId,
                        IsCoreSkill = false,
                        CreateAt = DateTime.UtcNow
                    };
                    _context.StudentSkills.Add(skill);

                    var progress = new StudentSkillProgress
                    {
                        ProgressID = Guid.NewGuid().ToString(),
                        UserID = studentId,
                        SkillID = skill.SkillID,
                        ProficiencyScore = 0,
                        SkillLevel = 1,
                        LastUpdated = DateTime.UtcNow,
                        Attempts = 0,
                        SuccessRate = 0
                    };
                    _context.StudentSkillProgresses.Add(progress);
                }

                // Optionally initialize StudentSkillProgress for all students in a class
                // (e.g., if generating for a class-specific quiz)
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                var mapper = _mapper.Map<List<StudentSkillVM>>(newSkills);
                return (string.Empty, mapper);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ($"Failed to generate skill from topic: {ex.Message} \n {ex.InnerException}", null);
            }
        }

        private async Task<List<GeneratedSkill>> CallGeminiApiAsync(string prompt)
        {
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                generationConfig = new
                {
                    temperature = 0.8,
                    maxOutputTokens = 200
                }
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
                throw new Exception("Error call API AI: No valid response received.");

            var generatedText = responseData?.candidates[0].content?.parts[0]?.text?.ToString();
            return ParseSkills(generatedText);
        }

        private List<GeneratedSkill> ParseSkills(string responseText)
        {
            var cleanedText = responseText.Replace("```json", string.Empty).Replace("```", string.Empty).Trim();

            // Nếu Gemini trả về 1 object → đưa vào List luôn
            if (cleanedText.StartsWith("{"))
            {
                var singleSkill = JsonConvert.DeserializeObject<GeneratedSkill>(cleanedText);
                return singleSkill != null ? new List<GeneratedSkill> { singleSkill } : new List<GeneratedSkill>();
            }
            // Nếu Gemini trả về array []
            else if (cleanedText.StartsWith("["))
            {
                var skillList = JsonConvert.DeserializeObject<List<GeneratedSkill>>(cleanedText);
                return skillList ?? new List<GeneratedSkill>();
            }

            throw new Exception("Generated text is not a valid JSON format for skills.");
        }
    }
}
