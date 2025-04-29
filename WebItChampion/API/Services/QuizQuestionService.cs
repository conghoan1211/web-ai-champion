using API.Models;
using Microsoft.EntityFrameworkCore;
using API.ViewModels;
using AutoMapper;
using API.Helper;

namespace API.Services
{
    public interface IQuizQuestionService
    {
        public Task<(string, List<QuizQuestionVM>?)> GetListByQuizId(string quizId);
        public Task<(string, QuizQuestionVM?)> GetOneQuestionById(string quizQuestionId);
    }

    public class QuizQuestionService : IQuizQuestionService
    {
        private readonly Sep490Context _context;
        private readonly IMapper _mapper;

        public QuizQuestionService(Sep490Context context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<(string, List<QuizQuestionVM>?)> GetListByQuizId(string quizId)
        {
            if (string.IsNullOrEmpty(quizId)) return ("Quiz Id was not found!", null);

            var list = await _context.QuizQuestions.Include(x=> x.Quiz)
                .Where(x => x.QuizID == quizId && x.Quiz.Status != (int)QuizStatus.Deleted).ToListAsync();
            if (list.Count == 0) return ("Quiz is not available!", null);

            var mapper = _mapper.Map<List<QuizQuestionVM>>(list);
            return ("", mapper);
        }   

        public async Task<(string, QuizQuestionVM?)> GetOneQuestionById(string quizQuestionId)
        {
            if (string.IsNullOrEmpty(quizQuestionId)) return ("Quiz Question Id was not found!", null);

            var quizQuestion = await _context.QuizQuestions.FirstOrDefaultAsync(x => x.QuestionID == quizQuestionId);
            if (quizQuestion == null) return ("Quiz Question is not available!", null);

            var mapper = _mapper.Map<QuizQuestionVM>(quizQuestion);
            return ("", mapper);
        }

    }
}
