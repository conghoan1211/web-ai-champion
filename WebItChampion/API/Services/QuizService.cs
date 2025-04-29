using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public interface IQuizService
    {
        public Task<(string, List<QuizVM>?)> GetListByTopicId(List<int> topicId, int? difficulty);
        public Task<(string, QuizVM?)> GetByQuizId(string quizId);
        public Task<(string, List<QuizVM>?)> GetAll();
        public Task<(string, QuizVM?)> GetByUserId(string userId);
    }
    public class QuizService : IQuizService
    {
        private readonly Sep490Context _context;
        private readonly IMapper _mapper;

        public QuizService(Sep490Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(string, List<QuizVM>?)> GetListByTopicId(List<int> topicId, int? difficulty)
        {
            if (topicId.Count <= 0) return ("Topic Id was not found!", null);

            var list = await _context.Quizzes
                .Where(x => topicId.Contains(x.TopicID.Value) && (!difficulty.HasValue || x.DifficultyLevel == difficulty)
                && x.Status != (int)QuizStatus.Deleted).ToListAsync();
            if (list.Count == 0) return ("Topic is not available!", null);

            var mapper = _mapper.Map<List<QuizVM>>(list);
            return ("", mapper);
        }

        public async Task<(string, QuizVM?)> GetByQuizId(string quizId)
        {
            if (string.IsNullOrEmpty(quizId)) return ("Quiz Id was not found!", null);

            var quiz = await _context.Quizzes.FirstOrDefaultAsync(x => x.QuizID == quizId 
                && x.Status != (int)QuizStatus.Deleted);
            if (quiz == null) return ("Quiz is not available!", null);

            var mapper = _mapper.Map<QuizVM>(quiz);
            return ("", mapper);
        }

        public async Task<(string, List<QuizVM>?)> GetAll()
        {
            var list = await _context.Quizzes.ToListAsync();
            if (list.Count == 0) return ("No quiz found!", null);

            var mapper = _mapper.Map<List<QuizVM>>(list);
            return ("", mapper);
        }

        public async Task<(string, QuizVM?)> GetByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ("User Id was not found!", null);

            var quiz = await _context.Quizzes.FirstOrDefaultAsync(x => x.UserID == userId);
            if (quiz == null) return ("No quiz found!", null);

            var mapper = _mapper.Map<QuizVM>(quiz);
            return ("", mapper);
        }
    }
}
